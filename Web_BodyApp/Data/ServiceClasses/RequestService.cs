using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Web_BodyApp.Data.AssistClasses;
using Web_BodyApp.Data.AssistClasses.StatusClasses;
using Web_BodyApp.Data.Models.NutritionData;

namespace Web_BodyApp.Data.ServiceClasses
{
    public class RequestService
    {
        private readonly HttpClient _http;
        private readonly string _baseURL = "http://bodyapp-api/api/";
        private readonly CompareLogic _compareLogic = new(new() { MaxDifferences = 100 });

        public RequestService(HttpClient httpClient) { _http = httpClient; }

        #region PATCH related
        public ComparisonResult CompareObjects<TModel>(TModel object1, TModel object2) where TModel : class
        {
            return _compareLogic.Compare(object1, object2);
        }

        private static string? GenerateOperationString(object beforeChange, object afterChange)
        {
            // Solo implementamos 3 operaciones ya que son las únicas que vamos a estar utilizando
            if (beforeChange == null && afterChange != null) return "add";
            if (beforeChange != null && afterChange == null) return "remove";
            if (beforeChange != null && afterChange != null && beforeChange != afterChange) return "replace";
            return null;
        }

        private static string GeneratePatchPathString(string rawPathString)
        {
            List<string> pathArray = rawPathString.Split('.').ToList();
            if (pathArray.Contains("Value")) pathArray.Remove("Value");
            for (int i = 0; i < pathArray.Count; i++)
            {
                if (pathArray[i].EndsWith("]"))
                {
                    pathArray[i] = pathArray[i].StartsWith("[")
                        ? pathArray[i].Replace("[", "").Replace("]", "")
                        : pathArray[i].Replace("[", "/").Replace("]", "");
                }
            }
            return "/" + string.Join('/', pathArray);
        }

        private static void ListDeleteDifferenceProcess(List<Difference> diffList, string propertyName, List<PatchStringCreationData> patchOperations)
        {
            List<Difference>? listDelete = diffList.Where(x => x.PropertyName.EndsWith(propertyName))?.ToList();
            if (listDelete?.Count > 0)
            {
                for (int i = 0; i < listDelete.Count; i++)
                {
                    Difference diff = listDelete[i];

                    var obj1Val = diff.Object1Value;
                    var obj2Val = diff.Object2Value;
                    if (obj1Val != obj2Val)
                    {
                        // Si se elimina un objeto de la lista, pero sigue habiendo lista
                        if (int.TryParse(obj1Val, out _) && int.TryParse(obj2Val, out _))
                        {
                            // Si es un elemento dentro de la lista
                            // en qué situación se genera
                            List<Difference> toDeleteDiffList = diffList.Where(x => x.PropertyName.StartsWith($"{diff.PropertyName}[")).ToList();
                            if (toDeleteDiffList.Count > 0)
                            {
                                int index = int.Parse(toDeleteDiffList[0].PropertyName.Substring(diff.PropertyName.Length + 1, 1));

                                PatchStringCreationData tempPatchData = new()
                                {
                                    op = "remove",
                                    path = GeneratePatchPathString(diff.PropertyName) + "/" + index
                                };
                                patchOperations.Add(tempPatchData);

                                foreach (Difference toDeleteDiff in toDeleteDiffList) { diffList.Remove(toDeleteDiff); }
                            }
                            else
                            {
                                int? removedIndex = null;

                                if (propertyName == "MealIntake")
                                {
                                    List<MealIntake> before = (List<MealIntake>)diff.Object1;
                                    List<MealIntake> after = (List<MealIntake>)diff.Object2;
                                    removedIndex = before.FindIndex(item1 => !after.Any(item2 => item2.MealId == item1.MealId));
                                }
                                else
                                {
                                    List<FoodData> before = (List<FoodData>)diff.Object1;
                                    List<FoodData> after = (List<FoodData>)diff.Object2;
                                    removedIndex = before.FindIndex(item1 => !after.Any(item2 => item2.Id == item1.Id));
                                }

                                if (removedIndex != null)
                                {
                                    PatchStringCreationData tempPatchData = new()
                                    {
                                        op = "remove",
                                        path = GeneratePatchPathString(diff.PropertyName) + "/" + removedIndex
                                    };
                                    patchOperations.Add(tempPatchData);
                                }
                            }
                        }
                        else
                        {
                            // Si se elimina un objeto de la lista, pero no sigue habiendo lista
                            if (obj2Val == "(null)") // Se ha eliminado el listado
                            {
                                if (diff.PropertyName.EndsWith(propertyName))
                                {
                                    PatchStringCreationData tempPatchData = new()
                                    {
                                        op = "remove",
                                        path = GeneratePatchPathString(diff.PropertyName)
                                    };
                                    patchOperations.Add(tempPatchData);
                                }
                            }
                        }
                    }

                    diffList.Remove(diff);
                }
            }
        }

        public List<PatchStringCreationData> GetPatchStringCreationDataList(List<Difference> diffList)
        {
            List<PatchStringCreationData> patchOperations = new();


            ListDeleteDifferenceProcess(diffList, "MealIntake", patchOperations);
            ListDeleteDifferenceProcess(diffList, "FoodIntake", patchOperations);


            foreach (Difference diff in diffList)
            {
                if (diff.PropertyName != "DayTimeIntakes" && diff.PropertyName != "Id")
                {
                    PatchStringCreationData tempPatchData = new()
                    {
                        op = GenerateOperationString(diff.Object1, diff.Object2)!,
                        path = GeneratePatchPathString(diff.PropertyName)
                    };
                    if (tempPatchData.op == null) continue;
                    if (tempPatchData.op != "remove") { tempPatchData.value = diff.Object2; }

                    patchOperations.Add(tempPatchData);
                }
            }

            return patchOperations;
        }

        public StringContent GenerateStringContentFromComparisonClass(List<Difference> differenceList)
        {
            List<PatchStringCreationData> patchOperations = GetPatchStringCreationDataList(differenceList);

            // Retorno
            return new StringContent(
                JsonConvert.SerializeObject(patchOperations),
                Encoding.UTF8,
                "application/json"
            );
        }


        public async Task<HttpResponseMessage> MakePatchPetition(string urlEnd, StringContent patchData)
        {
            try
            {
                HttpResponseMessage responseMsg = await _http.PatchAsync($"{_baseURL}{urlEnd}", patchData);
                return responseMsg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<HttpResponseMessage> ApplyChangesWithPatch(string endUrl, List<Difference> differenceList)
        {
            StringContent stringContent = GenerateStringContentFromComparisonClass(differenceList);

            HttpResponseMessage response = await MakePatchPetition(endUrl, stringContent);
            return response;
        }
        #endregion


        public async Task<HttpResponseMessage> MakeSingleDeletePetition(string urlEnd)
        {
            try
            {
                HttpResponseMessage responseMsg = await _http.DeleteAsync($"{_baseURL}{urlEnd}");
                return responseMsg;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<HttpResponseMessage> MakeMultipleDeletePetition(string endUrl, List<string> idsList)
        {
            try
            {
                HttpResponseMessage responseMsg = await _http.PostAsJsonAsync($"{_baseURL}{endUrl}", idsList);
                return responseMsg;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        // De Food
        public async Task<HttpResponseMessage> DeleteAllDayNutritionalData(HistoricalDataStatus hdStatus, string nutDataId, string histDataId, string userId)
        {
            try
            {
                await _http.DeleteAsync($"{_baseURL}NutritionalData/{nutDataId}"); // Como son los datos del día entero, eliminamos directamente el NutritionalData

                if (hdStatus.IsNutritionalData && !hdStatus.IsPhysicalData)
                {
                    await _http.DeleteAsync($"{_baseURL}HistoricalData/{histDataId}"); // Si lo único que hay en el HistoricalData es esto, eliminamos el HistoricalData
                    await _http.DeleteAsync($"{_baseURL}User/{userId}/historicalData/{histDataId}"); // Eliminamos la referencia al HistoricalData en el User // TODO CAMBIAR POR PATCH
                }
                else
                {
                    await _http.DeleteAsync($"{_baseURL}HistoricalData/{histDataId}/foodData/{nutDataId}"); // Si existe el PhysicalData, eliminamos el NutritionalData del HistoricalData // TODO CAMBIAR POR PATCH
                }

                return new HttpResponseMessage(HttpStatusCode.OK); // Si todas las operaciones se completaron con éxito, retornamos un HttpResponseMessage con un código de estado 200 (OK)
            }
            catch (Exception e)
            {
                // Si alguna de las operaciones falló, retornamos un HttpResponseMessage con un código de estado 500 (Internal Server Error)
                Console.WriteLine(e.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }


        public async Task<TModel?> MakePostPetition<TModel>(string urlEnd, object data) where TModel : class
        {
            try
            {
                HttpResponseMessage responseMsg = await _http.PostAsJsonAsync($"{_baseURL}{urlEnd}", data);

                if (responseMsg.IsSuccessStatusCode) return await responseMsg.Content.ReadFromJsonAsync<TModel>();
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
