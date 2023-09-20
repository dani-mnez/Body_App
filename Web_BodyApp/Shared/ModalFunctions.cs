using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Plotly.Blazor;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using Web_BodyApp.Data;
using Web_BodyApp.Data.AssistClasses;
using Web_BodyApp.Data.AssistClasses.StatusClasses;
using Web_BodyApp.Data.DTOs;
using Web_BodyApp.Data.Models;
using Web_BodyApp.Data.Models.NutritionData;
using Web_BodyApp.Data.Models.PhysicData;
using Web_BodyApp.Data.ServiceClasses;


namespace Web_BodyApp.Shared
{
    public class ModalFunctions : ComponentBase
    {
        [Inject] protected RequestService Request { get; set; }
        [Inject] protected UserStateService UserState { get; set; }
        [Inject] protected ReloadService Reload { get; set; }


        ///Para agregar datos en general ----------------------------------------------------------------- //
        protected HistoricalData? historicalData;
        protected int inputType { get; set; }
        private DateTime _selectedDate { get; set; }
        protected DateTime selectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                DateTime parsedDateTime = DateTime.Parse(value.ToString());

                // Obtenemos los datos de la fecha seleccionada
                historicalData = CreateOrSelectHistoricalData(parsedDateTime);
                if (inputType == 2)
                {
                    tempNutritionalData = CreateOrSelectNutritionalData();
                    dayTimeIntakesDict = CreateOrSelectIntakeInfo();

                    tempFoodData = ResetFoodData();
                    tempFoodServing = new();
                }

                validDate = CheckValidDate(parsedDateTime.Date);
                if (historicalData!.NutritionalData?.DayTimeIntakes != null && validDate) intakeDayTime = 1;
                _selectedDate = value;
            }
        }


        /// Para PhysicalData ----------------------------------------------------------------- //
        protected Dictionary<int, List<FormInputInfo>> methodToFormInputs { get; set; } = new();
        protected List<FormInputInfo> circInputList { get; set; } = new();
        protected PhysicalData tempPhysicalData { get; set; }


        /// Para NutritionalData ----------------------------------------------------------------- //
        protected bool existingHistoricalData { get; set; }
        protected bool existingNutritionalData { get; set; }
        protected NutritionalData tempNutritionalData { get; set; }
        protected FoodData tempFoodData { get; set; } = new();
        protected FoodServing tempFoodServing { get; set; } = new();
        private int _intakeDayTime { get; set; }
        protected int intakeDayTime
        {
            get
            {
                return _intakeDayTime;
            }
            set
            {
                if (_intakeDayTime == 0 && _intakeDayTime != value)
                {
                    tempNutritionalData = CreateOrSelectNutritionalData();
                }
                else if (_intakeDayTime != 0 && value == 0)
                {
                    tempNutritionalData = CreateOrSelectNutritionalData();
                    tempFoodData = ResetFoodData();
                    tempFoodServing = new();
                }
                _intakeDayTime = value;
            }
        }
        protected Dictionary<int, DayTimeIntakes> dayTimeIntakesDict { get; set; } = new();

        protected int selectedMealToAddIndex { get; set; }
        protected int selectedMethodToAddNutritionalInfo { get; set; }


        /// Para Meal ----------------------------------------------------------------- //
        protected Meal tempMeal { get; set; } = new();



        public async Task HandleValidSubmit(int inputType, Func<Task> closeAction)
        {
            // hData es el HISTORICALDATA seleccionado al cambiar de fecha
            bool createNewHistoricalData = historicalData?.Id == null;

            // Añadimos los datos a la BDD
            if (inputType == 0 || inputType == 1)
            {
                await AddPhysicalData(createNewHistoricalData);
            }
            else if (inputType == 2)
            {
                await AddNutritionalData(createNewHistoricalData);
            }
            else if (inputType == 3)
            {
                await AddMealData();
            }

            if (Reload.Reload != null) Reload.Reload();
            if (Reload.ReloadAsync() != null) await Reload.ReloadAsync();

            await closeAction();
        }

        #region PhysicalData
        private async Task AddPhysicalData(bool createNewHistoricalData)
        {
            HistoricalData historicalDataToPush = historicalData!.GetCopy();

            // Propiedades computadas
            CalcImcAndFatPerc();

            // Computamos las circunferencias
            if (tempPhysicalData?.BodyMeasure?.Circunferences != null) { ProcessCircunferenceData(); }

            // Calculamos la composición de peso y el porcentaje de músculo
            if (tempPhysicalData?.Computed.BodyStats?.FatPerc != null) { ProcessWeightComposition(); }


            /// BDD
            // Generamos en la BDD el objeto PhysicalData
            PhysicalData? physicalDataDevuelta = await Request.MakePostPetition<PhysicalData>("PhysicalData", tempPhysicalData!);

            if (physicalDataDevuelta != null)
            {
                if (createNewHistoricalData)
                {
                    // Creamos el HistoricalData con la referencia de PhysicalData y lo generamos en la BDD
                    HistoricalDataDTO historicalDataDTOToSend = new() { Date = historicalDataToPush.Date, PhysicalData = physicalDataDevuelta.Id };
                    HistoricalDataDTO? resultHistoricalData = await Request.MakePostPetition<HistoricalDataDTO>("HistoricalData", historicalDataDTOToSend);

                    if (resultHistoricalData != null)
                    {
                        // Generamos el StringContent para pasar por patch
                        List<PatchStringCreationData> patchToSend = new() { new() { op = "add", path = $"/HistoricalData/{UserState.HistoricalData.Count - 1}", value = resultHistoricalData.Id } };
                        StringContent finalPatchToSend = new(JsonConvert.SerializeObject(patchToSend), Encoding.UTF8, "application/json");


                        // Enviamos el patch a la API
                        HttpResponseMessage addHistoricalDataOnUserResponse = await Request.MakePatchPetition($"User/{UserState.UserData.Id}", finalPatchToSend);

                        /// LOCAL
                        historicalDataToPush.PhysicalData = physicalDataDevuelta;
                        UserState.HistoricalData.Add(historicalDataToPush!);
                        UserState.HistoricalData = new ObservableCollection<HistoricalData>(UserState.HistoricalData.OrderBy(hd => hd.Date).ToList());
                    }
                }
                else
                {
                    // Generamos el StringContent para pasar por patch
                    List<PatchStringCreationData> patchToSend = new() { new() { op = "replace", path = $"/PhysicalData", value = physicalDataDevuelta.Id } };
                    StringContent finalPatchToSend = new(JsonConvert.SerializeObject(patchToSend), Encoding.UTF8, "application/json");

                    HttpResponseMessage addPhysicalDataOnHistoricalDataResponse = await Request.MakePatchPetition($"HistoricalData/{historicalDataToPush.Id}", finalPatchToSend);

                    /// LOCAL
                    UserState.HistoricalData.First(hd => hd.Id == historicalDataToPush.Id).PhysicalData = physicalDataDevuelta;
                }
            }
        }

        private void CalcImcAndFatPerc()
        {
            if (tempPhysicalData.Weight != 0)
            {
                tempPhysicalData.Computed.BodyStats.Imc = Calculus.ImcCalc(UserState!.UserData!.Height, tempPhysicalData.Weight);

                if (UserState.UserData.ActivityLevel != null && UserState.UserData.FatLooseRate != null)
                    tempPhysicalData.Computed.Nutrients = Calculus.MacrosObjetivo(UserState.UserData, tempPhysicalData.Weight);
            }

            if (tempPhysicalData?.BodyMeasure?.BodyFat?.Method != 0 && tempPhysicalData?.BodyMeasure?.BodyFat != null)
            {
                tempPhysicalData.Computed.BodyStats.FatPerc = Calculus.FatPercentageCalc(
                    UserState.UserData!,
                    tempPhysicalData.Weight,
                    tempPhysicalData.BodyMeasure.BodyFat
                );
            }
        }
        private void ProcessCircunferenceData()
        {
            Circunferences circunferences = tempPhysicalData.BodyMeasure!.Circunferences!;
            PropertyInfo[] properties = circunferences.GetType().GetProperties();
            List<double> values = properties.Select(p => (double)p.GetValue(circunferences)!).ToList();

            if (values.All(v => v == 0))
            {
                tempPhysicalData.BodyMeasure.Circunferences = null;
            }
            else if (values.All(v => v != 0))
            {
                tempPhysicalData.Computed.BodyStats.TotalCircunferences = values.Sum();
            }
        }
        private void ProcessWeightComposition()
        {
            tempPhysicalData.Computed.BodyStats.WeightComposition = Calculus.WeightCompositionCalc(
                UserState!.UserData!.Sex,
                tempPhysicalData.Weight,
                UserState.UserData.Height,
                (double)tempPhysicalData.Computed.BodyStats.FatPerc!
            );

            tempPhysicalData.Computed.BodyStats.MusclePerc = Calculus.MusclePercentageCalc(
                tempPhysicalData.Weight,
                (double)tempPhysicalData.Computed.BodyStats.WeightComposition.MuscleWeight
            );
        }
        #endregion

        #region NutritionalData
        private Dictionary<int, Tuple<List<FoodData>?, List<string>?>> GenUtilityDictionaries()
        {
            Dictionary<int, Tuple<List<FoodData>?, List<string>?>> tempDict = new(); // Utilidad para más adelante

            foreach (var (dayTime, intakeData) in dayTimeIntakesDict)
            {
                // Juntamos los datos de Food y Meal según su dayTime y agregamos al diccionario
                tempDict.Add(dayTime, new(
                    (intakeData.Type == 0 || intakeData.Type == 2) ? intakeData.FoodIntake : null, // Si hay FoodIntakes, los agregamos (como objeto)
                    (intakeData.Type == 1 || intakeData.Type == 2) ? intakeData.MealIntake!.Select(mi => mi.MealId!).ToList() : null //Agregamos las IDs de los MealIntakes (para la BDD)
                ));
            }

            return tempDict;
        }

        private DayTimeIntakes CreateDayTimeIntakes(DayTimeIntakes intakeData)
        {
            var (totalDayIntakesCals, totalDayIntakesMacros) = Calculus.CalculateDayTimeIntakesCalsMacros(intakeData, UserState.Meals.ToList());

            return new DayTimeIntakes
            {
                Type = Utils.CreateDayTimeIntakeType(intakeData.FoodIntake, intakeData.MealIntake),
                FoodIntake = intakeData.FoodIntake,
                MealIntake = intakeData.MealIntake,
                TotalDayTimeIntakesCalories = totalDayIntakesCals,
                TotalDayTimeIntakesMacros = totalDayIntakesMacros
            };
        }

        public static (NutritionalDataDTO, NutritionalData) DayTimeIntakesDictToNutDataDTO(Dictionary<int, DayTimeIntakes> dayTimeIntakesDict,
            List<string>? foodDataIdList, Dictionary<int, Tuple<List<FoodData>?, List<string>?>> tempDict)
        {
            Dictionary<int, DayTimeIntakesDTO>? tempDayTimeIntakesDictDTO = new();
            Dictionary<int, DayTimeIntakes>? tempDayTimeIntakesDict = new();

            int totalNutDataCalories = 0;
            FoodMacros totalNutDataMacros = new();
            int foodDataCounterInitIdx = 0;
            foreach ((int dayTime, DayTimeIntakes dayTimeIntakes) in dayTimeIntakesDict)
            {
                List<string>? tempFoodIntakeIdList = null;
                List<FoodData> tempFoodIntake = new();

                int? dayTimeFoodDataCount = tempDict[dayTime]?.Item1?.Count();
                if (dayTimeFoodDataCount > 0)
                {
                    tempFoodIntakeIdList = foodDataIdList!.Skip(foodDataCounterInitIdx).Take((int)dayTimeFoodDataCount).ToList();
                    tempFoodIntake = tempDict[dayTime]?.Item1?.Skip(foodDataCounterInitIdx).Take((int)dayTimeFoodDataCount).ToList();
                    foodDataCounterInitIdx += (int)dayTimeFoodDataCount;
                }
                DayTimeIntakesDTO tempDayTimeIntakesDTO = new()
                {
                    Type = Utils.CreateDayTimeIntakeType(dayTimeIntakes.FoodIntake, dayTimeIntakes.MealIntake),
                    FoodIntake = tempFoodIntakeIdList,
                    MealIntake = dayTimeIntakes.MealIntake,
                    TotalDayTimeIntakesCalories = dayTimeIntakes.TotalDayTimeIntakesCalories,
                    TotalDayTimeIntakesMacros = dayTimeIntakes.TotalDayTimeIntakesMacros
                };
                DayTimeIntakes tempDayTimeIntakes = new()
                {
                    Type = tempDayTimeIntakesDTO.Type,
                    FoodIntake = tempFoodIntake,
                    MealIntake = tempDayTimeIntakesDTO.MealIntake,
                    TotalDayTimeIntakesCalories = tempDayTimeIntakesDTO.TotalDayTimeIntakesCalories,
                    TotalDayTimeIntakesMacros = tempDayTimeIntakesDTO.TotalDayTimeIntakesMacros
                };
                tempDayTimeIntakesDictDTO.Add(dayTime, tempDayTimeIntakesDTO);
                tempDayTimeIntakesDict.Add(dayTime, tempDayTimeIntakes);

                // Hacemos el sumatorio de las calorías y macros totales
                totalNutDataCalories += dayTimeIntakes.TotalDayTimeIntakesCalories;

                totalNutDataMacros.Carbs += dayTimeIntakes.TotalDayTimeIntakesMacros.Carbs;
                totalNutDataMacros.Fats += dayTimeIntakes.TotalDayTimeIntakesMacros.Fats;
                totalNutDataMacros.Prots += dayTimeIntakes.TotalDayTimeIntakesMacros.Prots;
            }

            return (
                new NutritionalDataDTO()
                {
                    DayTimeIntakes = tempDayTimeIntakesDictDTO,
                    TotalNutDataCalories = totalNutDataCalories,
                    TotalNutDataMacros = totalNutDataMacros
                },
                new NutritionalData()
                {
                    DayTimeIntakes = tempDayTimeIntakesDict,
                    TotalNutDataCalories = totalNutDataCalories,
                    TotalNutDataMacros = totalNutDataMacros
                }
            );
        }

        public (Dictionary<int, List<FoodData>>, Dictionary<int, DayTimeIntakes>) DifferencesAnalisys(ComparisonResult comparison)
        {
            Dictionary<int, List<FoodData>> foodDataToAdd = new();
            Dictionary<int, DayTimeIntakes> fullDayTimeIntakeToAdd = new();

            if (!comparison.AreEqual)
            {
                foreach (var item in comparison.Differences)
                {
                    string propName = item.PropertyName;

                    bool foodIntakeMod = propName.EndsWith("FoodIntake");
                    bool newFullDayTimeIntakeToAdd = propName.StartsWith("DayTimeIntakes.[") && propName.EndsWith("].Value");


                    if (foodIntakeMod || newFullDayTimeIntakeToAdd)
                    {
                        int key = int.Parse(propName.Replace("[", "|").Replace("]", "|").Split("|")[1]);

                        if (foodIntakeMod)
                        {
                            int oldFoodDataItemsCount = int.Parse(item.Object1Value);
                            int newFoodDataItemsCount = int.Parse(item.Object2Value);
                            int newItems = newFoodDataItemsCount - oldFoodDataItemsCount;

                            List<FoodData> tempFoodData = (item.Object2 as List<FoodData>)!.Skip(oldFoodDataItemsCount).ToList();

                            if (foodDataToAdd.ContainsKey(key))
                            {
                                foodDataToAdd[key] = tempFoodData;
                            }
                            else
                            {
                                foodDataToAdd.Add(key, tempFoodData);
                            }
                        }

                        if (newFullDayTimeIntakeToAdd)
                        {
                            DayTimeIntakes newDayTimeIntakes = (item.Object2 as DayTimeIntakes)!;

                            if (newDayTimeIntakes.FoodIntake?.Count > 0)
                            {
                                if (foodDataToAdd.ContainsKey(key))
                                {
                                    foodDataToAdd[key] = newDayTimeIntakes.FoodIntake;
                                }
                                else
                                {
                                    foodDataToAdd.Add(key, newDayTimeIntakes.FoodIntake);
                                }
                            }
                            fullDayTimeIntakeToAdd.Add(key, newDayTimeIntakes);
                        }
                    }
                }
            }

            return (foodDataToAdd, fullDayTimeIntakeToAdd);
        }



        private NutritionalDataDTO GenDTO(NutritionalData localNutData, Dictionary<int, List<string>> tempDayTimeListFoodDataIdList)
        {
            Dictionary<int, DayTimeIntakesDTO> tempDayTimeIntakesDictDTO = new();
            foreach ((int dayTime, DayTimeIntakes intake) in localNutData.DayTimeIntakes!)
            {
                tempDayTimeIntakesDictDTO.Add(dayTime, intake.ToDTO());
            }
            foreach ((int dayTime, DayTimeIntakesDTO intake) in tempDayTimeIntakesDictDTO)
            {
                if (tempDayTimeListFoodDataIdList.ContainsKey(dayTime)) intake.FoodIntake?.AddRange(tempDayTimeListFoodDataIdList[dayTime]);
            }

            return new()
            {
                DayTimeIntakes = tempDayTimeIntakesDictDTO,
                TotalNutDataCalories = localNutData.TotalNutDataCalories,
                TotalNutDataMacros = localNutData.TotalNutDataMacros
            };
        }

        private Dictionary<int, List<string>> StructureIDs(Dictionary<int, List<FoodData>> foodDataToAdd, List<string>? foodDataIdsResponse)
        {
            Dictionary<int, List<string>> tempDayTimeListFoodDataIdList = new();
            int position = 0;
            foreach ((int dayTime, List<FoodData> foodDataObjectList) in foodDataToAdd)
            {
                int itemsToAddCount = foodDataObjectList.Count;
                tempDayTimeListFoodDataIdList.Add(dayTime, foodDataIdsResponse!.Skip(position).Take(itemsToAddCount).ToList());
                position += itemsToAddCount;
            }

            return tempDayTimeListFoodDataIdList;
        }

        private ComparisonResult ProcessedFinalComparison(NutritionalDataDTO original, NutritionalDataDTO modified)
        {
            ComparisonResult nutDataDTOComparisonResult = Request.CompareObjects<NutritionalDataDTO>(original, modified);
            for (int idx = 0; idx < nutDataDTOComparisonResult.Differences.Count - 1; idx++)
            {
                if (nutDataDTOComparisonResult.Differences[idx].PropertyName == "DayTimeIntakes"
                    || nutDataDTOComparisonResult.Differences[idx].PropertyName == "Id")
                {
                    nutDataDTOComparisonResult.Differences.RemoveAt(idx);
                    idx--;
                }
            }

            return nutDataDTOComparisonResult;
        }

        private void UpdateLocalDataWithIDs(ref NutritionalData localNutData, Dictionary<int, List<string>> tempDayTimeListFoodDataIdList)
        {
            foreach ((int dayTime, DayTimeIntakes intake) in localNutData.DayTimeIntakes)
            {
                if (intake.FoodIntake != null)
                {
                    int idx = 0;
                    foreach (FoodData foodData in intake.FoodIntake)
                    {
                        if (foodData.Id == null)
                        {
                            foodData.Id = tempDayTimeListFoodDataIdList[dayTime][idx];
                            idx++;
                        }
                    }
                }
            }
        }

        public async Task AddNutritionalData(bool createNewHistoricalData)
        {
            HistoricalData historicalDataToPush = historicalData!.GetCopy();
            NutritionalData localNutData = tempNutritionalData.Copy();

            string? nutDataId = (existingNutritionalData) ? localNutData.Id : null;

            if (intakeDayTime == 0)
            {
                // Si son datos nutricionales del día entero
                NutritionalDataDTO? returnedNutDataFromPost = await Request.MakePostPetition<NutritionalDataDTO>("NutritionalData", tempNutritionalData.Copy());

                if (returnedNutDataFromPost != null) nutDataId = returnedNutDataFromPost.Id!;
            }
            else
            {
                // Si son datos nutricionales de comidas pormenorizadas
                if (existingNutritionalData)
                {
                    // Actualizamos el total de calorías y macros del NutritionalData actualizado
                    (int newNutCals, FoodMacros newNutMacros) = Calculus.CalculateNutDataCalsMacros(localNutData, UserState.Meals.ToList());
                    localNutData.TotalNutDataCalories = newNutCals;
                    localNutData.TotalNutDataMacros = newNutMacros;


                    // Creamos un nutData 'virgen' para comparar posteriormente 
                    NutritionalData unmodifiedNutData = UserState.HistoricalData.First(hd => hd.Id == historicalDataToPush.Id && hd.NutritionalData!.Id == localNutData.Id).NutritionalData!;


                    // Se crea un nutDataDTO a partir del unmodifiedNutData (para compararlo con el otro más adelante)
                    NutritionalDataDTO unmodifiedNutDataDTO = unmodifiedNutData.ToDTO();


                    // Generamos utilidades sobre la comparación
                    ComparisonResult nutDataComparison = Request.CompareObjects(unmodifiedNutData, localNutData);
                    (Dictionary<int, List<FoodData>> foodDataToAdd,
                    Dictionary<int, DayTimeIntakes> fullDayTimeIntakeToAdd) = DifferencesAnalisys(nutDataComparison);


                    // Se generan en la BDD los FoodData nuevos
                    List<string>? foodDataIdsResponse = await Request.MakePostPetition<List<string>>("FoodData/multiple", foodDataToAdd.SelectMany(d => d.Value).ToList());


                    // Estructuramos los IDs de los FoodData recibidos
                    Dictionary<int, List<string>> tempDayTimeListFoodDataIdList = StructureIDs(foodDataToAdd, foodDataIdsResponse);


                    // Generamos el DTO con los datos nuevos
                    NutritionalDataDTO modNutDataDTO = GenDTO(localNutData, tempDayTimeListFoodDataIdList);


                    // Se comparan el modifiedNutDataDTO con el unmodifiedNutDataDTO (y procesamos los resultados)
                    ComparisonResult nutDataDTOComparisonResult = ProcessedFinalComparison(unmodifiedNutDataDTO, modNutDataDTO);

                    HttpResponseMessage updateNutritionalDataResponse = await Request.ApplyChangesWithPatch($"NutritionalData/{localNutData.Id}", nutDataDTOComparisonResult.Differences);


                    // Agregamos los IDs al objeto local
                    UpdateLocalDataWithIDs(ref localNutData, tempDayTimeListFoodDataIdList);
                }
                else
                {
                    // Generamos diccionarios de utilidad para hacer operaciones en masa a la BDD
                    Dictionary<int, Tuple<List<FoodData>?, List<string>?>> tempDict = GenUtilityDictionaries();

                    // Push de FoodData y agregamos el id a la lista
                    if (tempDict.Count > 0)
                    {
                        List<FoodData>? foodDataListToUpload = tempDict.Values.Where(d => d.Item1 != null).SelectMany(d => d.Item1!).ToList();
                        List<string>? foodDataIdsResponse = null;

                        if (foodDataListToUpload.Count > 0)
                        {
                            foodDataIdsResponse = await Request.MakePostPetition<List<string>>("FoodData/multiple", foodDataListToUpload);
                        }

                        // Generamos el nutritionalDataDTO y el nutritionalData a partir de DayTimeIntakesDTO (para poder generar el objeto en la BDD y agregar al objeto local)
                        (NutritionalDataDTO tempNutDataDTO, NutritionalData tempLocalNutData) = DayTimeIntakesDictToNutDataDTO(dayTimeIntakesDict!, foodDataIdsResponse, tempDict);


                        NutritionalDataDTO? newNutDataDTOReturned = await Request.MakePostPetition<NutritionalDataDTO>("NutritionalData", tempNutDataDTO);
                        if (newNutDataDTOReturned != null) { nutDataId = newNutDataDTOReturned.Id; }

                        // Para local
                        tempLocalNutData.Id = nutDataId;
                        localNutData = tempLocalNutData;
                    }
                }
            }


            if (createNewHistoricalData)
            {
                HistoricalDataDTO historicalDataDTO = new()
                {
                    Date = historicalDataToPush.Date,
                    NutritionalData = nutDataId
                };

                HistoricalDataDTO? resultNewHistoricalData = await Request.MakePostPetition<HistoricalDataDTO>("HistoricalData", historicalDataDTO);

                if (resultNewHistoricalData != null)
                {
                    List<PatchStringCreationData> patchToSend = new() { new() { op = "add", path = $"/HistoricalData/{UserState.UnorderedHistoricalDataIds.Count - 1}", value = resultNewHistoricalData!.Id } };
                    StringContent finalPatchToSend = new(JsonConvert.SerializeObject(patchToSend), Encoding.UTF8, "application/json");

                    // Agregamos el Id del HistoricalData al listado del User en la BDD
                    HttpResponseMessage addHistDataIdToUserResponse = await Request.MakePatchPetition($"User/{UserState.UserData.Id}", finalPatchToSend);


                    // LOCAL
                    HistoricalData localHistoricalData = new()
                    {
                        Date = historicalDataToPush.Date,
                        NutritionalData = localNutData
                    };
                    UserState.HistoricalData.Add(localHistoricalData);
                    UserState.HistoricalData = new ObservableCollection<HistoricalData>(UserState.HistoricalData.OrderBy(h => h.Date));
                }
            }
            else
            {
                if (!existingNutritionalData)
                {
                    List<PatchStringCreationData> patchToSend = new() { new() { op = "replace", path = "/NutritionalData", value = nutDataId } };
                    StringContent finalPatchToSend = new(JsonConvert.SerializeObject(patchToSend), Encoding.UTF8, "application/json");

                    // Añadimos una referencia al NutritionalData en HistoricalData
                    HttpResponseMessage addHistDataIdToUserResponse = await Request.MakePatchPetition($"HistoricalData/{historicalDataToPush.Id}", finalPatchToSend);
                }

                // LOCAL
                UserState.HistoricalData.FirstOrDefault(hd => hd.Id == historicalDataToPush.Id)!.NutritionalData = localNutData;
            }
        }


        public void AddNewFood()
        {
            bool dayTimeExists = dayTimeIntakesDict.ContainsKey(intakeDayTime);

            if (dayTimeExists)
            {
                DayTimeIntakes dayTimeIntakes = dayTimeIntakesDict[intakeDayTime];
                if (selectedMethodToAddNutritionalInfo == 0)
                {
                    // Hacemos las modificaciones necesarias
                    if (dayTimeIntakes.Type == 1)
                    {
                        dayTimeIntakes.Type = 2;
                        dayTimeIntakes.FoodIntake = new List<FoodData>();
                    }

                    // Agregamos los nuevos datos al objeto temporal
                    dayTimeIntakes.FoodIntake!.Add(tempFoodData.GetCopy());

                    // Reset de las propiedades
                    tempFoodData = ResetFoodData();
                }
                else
                {
                    // Hacemos las modificaciones necesarias
                    if (dayTimeIntakes.Type == 0)
                    {
                        dayTimeIntakes.Type = 2;
                        dayTimeIntakes.MealIntake = new List<MealIntake>();
                    }

                    // Agregamos los nuevos datos al objeto temporal
                    MealIntake tempMealIntake = GenerateNewMealIntake(UserState.Meals[selectedMealToAddIndex - 1], tempFoodServing);
                    dayTimeIntakes.MealIntake!.Add(tempMealIntake);

                    // Reset de las propiedades
                    tempFoodServing = new();
                    selectedMealToAddIndex = 0;
                }

                // Recalculamos las macros del DayTimesIntake y las actualizamos
                var (updatedDayIntakeCals, updatedDayIntakeMacros) = Calculus.CalculateDayTimeIntakesCalsMacros(dayTimeIntakes, UserState.Meals.ToList());

                dayTimeIntakes.TotalDayTimeIntakesCalories = updatedDayIntakeCals;
                dayTimeIntakes.TotalDayTimeIntakesMacros = updatedDayIntakeMacros;
            }
            else
            {
                if (selectedMethodToAddNutritionalInfo == 0)
                {
                    /// Input de FoodIntake
                    // Creamos el DayTimeIntakes y lo añadimos al diccionario
                    DayTimeIntakes tempDayTimeIntake = new()
                    {
                        Type = 0,
                        FoodIntake = new List<FoodData> { tempFoodData.GetCopy() },
                        TotalDayTimeIntakesCalories = tempFoodData.FoodCalories,
                        TotalDayTimeIntakesMacros = tempFoodData.FoodMacros
                    };
                    dayTimeIntakesDict.Add(intakeDayTime, tempDayTimeIntake);

                    // Reset de las propiedades
                    tempFoodData = ResetFoodData();
                }
                else
                {
                    /// Input de MealIntake
                    // Creamos el DayTimeIntakes y lo añadimos al diccionario
                    DayTimeIntakes tempDayTimeIntake = GenerateNewDayTimesIntake(
                        UserState.Meals[selectedMealToAddIndex - 1], // El -1 es porque en el select, la opción 0 es un mensaje informativo
                        tempFoodServing
                    );
                    dayTimeIntakesDict.Add(intakeDayTime, tempDayTimeIntake);

                    // Reset de las propiedades
                    tempFoodServing = new();
                    selectedMealToAddIndex = 0;
                }
            }
        }


        #region Gestión previa a AddNewFood
        // Generación de MealIntake (para NUEVA key en el diccionario)
        private static MealIntake GenerateNewMealIntake(Meal usedMeal, FoodServing usedServing)
        {
            double mealMultiplier = Calculus.CalculateMealServingMultiplier(usedMeal, usedServing);
            return new()
            {
                MealId = usedMeal.Id,
                MealServing = usedServing,
                TotalMealIntakeCalories = (int)(usedMeal.TotalCalories * mealMultiplier),
                TotalMealIntakeMacros = new FoodMacros()
                {
                    Carbs = (int)(usedMeal.TotalMealMacros.Carbs * mealMultiplier),
                    Prots = (int)(usedMeal.TotalMealMacros.Prots * mealMultiplier),
                    Fats = (int)(usedMeal.TotalMealMacros.Fats * mealMultiplier)
                }
            };
        }

        // Generación de DayTimeIntakes (para NUEVA key en el diccionario)
        private DayTimeIntakes GenerateNewDayTimesIntake(Meal usedMeal, FoodServing usedServing)
        {
            MealIntake tempMealIntake = GenerateNewMealIntake(usedMeal, usedServing);
            return new()
            {
                Type = 1,
                MealIntake = new List<MealIntake> { tempMealIntake },
                TotalDayTimeIntakesCalories = tempMealIntake.TotalMealIntakeCalories,
                TotalDayTimeIntakesMacros = tempMealIntake.TotalMealIntakeMacros
            };
        }

        // Reset de las propiedades
        protected static FoodData ResetFoodData()
        {
            return new FoodData()
            {
                FoodMacros = new FoodMacros()
                {
                    Carbs = 0,
                    Prots = 0,
                    Fats = 0
                },
                FoodCalories = 0
            };
        }

        // Gestión de las listas de comidas y alimentos (para inputType 2))
        protected void DeleteElementFromIntakeDict((int dayTimeDictKey, int type, int elementIndex) parameters, Dictionary<int, DayTimeIntakes> dayTimeIntakesDict, NutritionalData formNutData)
        {
            DayTimeIntakeDictStatus dayTimeIntakeDictStatus = Utils.GetIntakeDictStatus(dayTimeIntakesDict);
            DayTimeIntakeStatus selectedDayTimeIntakeStatus = dayTimeIntakeDictStatus.DayTimeIntakeStatus[parameters.dayTimeDictKey]!;

            if (parameters.type == 0)
                HandleIntakeDeletion(parameters.elementIndex, parameters.dayTimeDictKey, true, selectedDayTimeIntakeStatus.LastFoodIntake ?? true, dayTimeIntakesDict); // Si es FoodIntake
            else
                HandleIntakeDeletion(parameters.elementIndex, parameters.dayTimeDictKey, false, selectedDayTimeIntakeStatus.LastMealIntake ?? true, dayTimeIntakesDict); // Si es MealIntake


            // Si es el unico elemento de dayTimeStatus -> Se elimina la (key, val)
            if ((selectedDayTimeIntakeStatus.LastMealIntake ?? true) && (selectedDayTimeIntakeStatus.LastFoodIntake ?? true))
            {
                dayTimeIntakesDict.Remove(parameters.dayTimeDictKey);
            }
            else
            {
                var (dayTimeTotalCalories, dayTimeTotalMacros) = Calculus.CalculateDayTimeIntakesCalsMacros(dayTimeIntakesDict[parameters.dayTimeDictKey], UserState.Meals.ToList());
                dayTimeIntakesDict[parameters.dayTimeDictKey].TotalDayTimeIntakesCalories = dayTimeTotalCalories;
                dayTimeIntakesDict[parameters.dayTimeDictKey].TotalDayTimeIntakesMacros = dayTimeTotalMacros;
            }


            // Si es el unico elemento del diccionario -> Se resetea el diccionario
            if (!dayTimeIntakeDictStatus.LastDayTimeKey)
            {
                var (nutDataTotalCalories, nutDataTotalMacros) = Calculus.CalculateNutDataCalsMacros(formNutData, UserState.Meals.ToList());
                formNutData.TotalNutDataCalories = nutDataTotalCalories;
                formNutData.TotalNutDataMacros = nutDataTotalMacros;
            }
        }

        private static void HandleIntakeDeletion(int elemIdx, int elemDictKey, bool isFoodIntake, bool isLastIntakeListElement, Dictionary<int, DayTimeIntakes> dayTimeIntakesDict)
        {
            // Si es el ultimo elemento de la lista de intakes (ya sea Meal o Food)
            if (isLastIntakeListElement)
            {
                if (isFoodIntake)
                    dayTimeIntakesDict[elemDictKey].FoodIntake = null;
                else
                    dayTimeIntakesDict[elemDictKey].MealIntake = null;
            }
            else
            {
                if (isFoodIntake)
                    dayTimeIntakesDict[elemDictKey].FoodIntake!.RemoveAt(elemIdx);
                else
                    dayTimeIntakesDict[elemDictKey].MealIntake!.RemoveAt(elemIdx);
            }
        }
        #endregion

        #endregion

        #region MealData
        public async Task AddMealData()
        {
            Meal mealDataToAdd = tempMeal.GetCopy();

            /// BDD
            // Generamos el documento Meal
            Meal? mealFromPostResponse = await Request.MakePostPetition<Meal>("Meal", mealDataToAdd);

            if (mealFromPostResponse != null)
            {
                // OJO Todos los patch deben serializarse dentro de un List o la petición estará mal formada
                List<PatchStringCreationData> patchList = new()
                {
                    new() { op = "add", path = $"/CustomMeals/{UserState.Meals.Count - 1}", value = mealFromPostResponse.Id }
                };
                // Generamos el patch y editamos el documento User para incorporar el Id de Meal
                StringContent finalPatchToSend = new(JsonConvert.SerializeObject(patchList), Encoding.UTF8, "application/json");
                await Request.MakePatchPetition($"User/{UserState.UserData.Id}", finalPatchToSend);
            }


            /// LOCAL
            UserState.Meals.Add(mealDataToAdd!);
        }
        #endregion


        #region Auxiliares
        // Validaciones de formulario
        protected bool validDate { get; set; }
        protected List<DateTime> usedNutritionalDates { get; set; } = new();
        protected List<DateTime> usedPhysicalDates { get; set; } = new();


        // Validación de fecha
        private bool CheckValidDate(DateTime dateToCheck)
        {
            bool result = true;
            if (inputType == 0 || inputType == 1)
            {
                result = !usedPhysicalDates.Contains(dateToCheck);
            }
            else if (inputType == 2)
            {
                result = !usedNutritionalDates.Contains(dateToCheck) ||
                         (tempNutritionalData.Id != null && tempNutritionalData.DayTimeIntakes != null);
            }
            return result;

        }


        // Generación de datos de uso de fechas
        protected (List<DateTime>, List<DateTime>) GenerateUsedDatesInfo(List<HistoricalData> historicalData)
        {
            List<DateTime> nutritionalDates = new();
            List<DateTime> physicalDates = new();

            foreach (HistoricalData hData in historicalData)
            {
                if (hData.PhysicalData != null) physicalDates.Add(hData.Date.Date);
                if (hData.NutritionalData != null) nutritionalDates.Add(hData.Date.Date);
            }
            return (physicalDates, nutritionalDates);
        }


        /// Creación o selección de: 
        // HistoricalData
        protected HistoricalData CreateOrSelectHistoricalData(DateTime date)
        {
            HistoricalData? tempHistData = UserState.HistoricalData.FirstOrDefault(hd => hd.Date.Date == date.Date)?.GetCopy();
            if (tempHistData != null)
            {
                existingHistoricalData = true;
                return tempHistData;
            }
            else
            {
                existingNutritionalData = false;
                return new HistoricalData() { Date = date };
            }
        }


        // NutritionalData
        protected NutritionalData CreateOrSelectNutritionalData()
        {
            NutritionalData? tempNutritionalData = historicalData?.NutritionalData;
            if (tempNutritionalData != null)
            {
                existingNutritionalData = true;
                return tempNutritionalData;
            }
            else
            {
                existingNutritionalData = false;
                return new NutritionalData { TotalNutDataCalories = 0, TotalNutDataMacros = new FoodMacros() };
            }
        }


        // IntakeInfo
        protected Dictionary<int, DayTimeIntakes> CreateOrSelectIntakeInfo()
        {
            return tempNutritionalData?.DayTimeIntakes ?? new();
        }
        #endregion
    }
}
