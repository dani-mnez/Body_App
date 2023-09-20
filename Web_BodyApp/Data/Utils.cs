using System.Text.Json;
using Web_BodyApp.Data.AssistClasses;
using Web_BodyApp.Data.AssistClasses.StatusClasses;
using Web_BodyApp.Data.DTOs;
using Web_BodyApp.Data.Models;
using Web_BodyApp.Data.Models.NutritionData;
using Web_BodyApp.Data.Models.PhysicData;

namespace Web_BodyApp.Data
{
    public class Utils
    {
        #region Modal.razor
        public static PhysicalData NewPhysicalData(int inputType)
        {
            PhysicalData basePhysicalData = new()
            {
                Weight = 0,
                BodyMeasure = null,
                Computed = new ComputedData
                {
                    BodyStats = new BodyStats
                    {
                        Imc = 0,
                        WeightComposition = null
                    },
                    Nutrients = new Nutrients
                    {
                        DiaryTMB = 0,
                        MaintainKcals = 0,
                        FatLossKcals = 0,
                        Macros = new Macros
                        {
                            PercProt = 0,
                            PercCarbs = 0,
                            PercFat = 0,
                            GramsProt = 0,
                            GramsCarbs = 0,
                            GramsFat = 0
                        }
                    }
                }
            };

            if (inputType == 1)
            {
                basePhysicalData.BodyMeasure = new BodyMeasure
                {
                    BodyFat = new BodyFat
                    {
                        Method = 0,
                        Measures = new Measures()
                    },
                    Circunferences = new Circunferences
                    {
                        LeftBicep = 0,
                        RightBicep = 0,
                        Waist = 0,
                        Hip = 0,
                        LeftThigh = 0,
                        RightThigh = 0
                    }
                };
                basePhysicalData.Computed.BodyStats.WeightComposition = new WeightComposition
                {
                    FatWeight = 0,
                    MuscleWeight = 0,
                    BoneWeight = 0,
                    ResidualWeight = 0
                };
            };
            return basePhysicalData;
        }

        public static Dictionary<int, List<FormInputInfo>> NewFatInputData(PhysicalData physicicalData)
        {
            FormInputInfo midAxilary = new()
            {
                Id = "bodyFatMeasureMidaxillary",
                Label = "Media axila (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.MidAxilary,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.MidAxilary = value
            };
            FormInputInfo chest = new()
            {
                Id = "bodyFatMeasureChest",
                Label = "Pecho (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.Chest,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.Chest = value
            };
            FormInputInfo abdominal = new()
            {
                Id = "bodyFatMeasureAbdominal",
                Label = "Abdominal (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.Abdominal,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.Abdominal = value
            };
            FormInputInfo thigh = new()
            {
                Id = "bodyFatMeasureThigh",
                Label = "Muslo (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.Thigh,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.Thigh = value
            };
            FormInputInfo suprailiac = new()
            {
                Id = "bodyFatMeasureSuprailiac",
                Label = "Suprailíaco (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.Suprailiac,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.Suprailiac = value
            };
            FormInputInfo subscapular = new()
            {
                Id = "bodyFatMeasureSubscapular",
                Label = "Subescapular (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.Subscapular,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.Subscapular = value
            };
            FormInputInfo tricep = new()
            {
                Id = "bodyFatMeasureTriceps",
                Label = "Tríceps (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.Tricep,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.Tricep = value
            };
            FormInputInfo bicep = new()
            {
                Id = "bodyFatMeasureBicep",
                Label = "Bíceps (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.Bicep,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.Bicep = value
            };
            FormInputInfo lowerBack = new()
            {
                Id = "bodyFatMeasureLowerBack",
                Label = "Baja espalda (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.LowerBack,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.LowerBack = value
            };
            FormInputInfo calf = new()
            {
                Id = "bodyFatMeasureCalf",
                Label = "Pantorrilla (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.Calf,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.Calf = value
            };

            FormInputInfo tapeNeck = new()
            {
                Id = "bodyFatMeasureTapeNeck",
                Label = "Cuello (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.TapeNeck,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.TapeNeck = value,
            };
            FormInputInfo tapeAbdomen = new()
            {
                Id = "bodyFatMeasureTapeAbdomen",
                Label = "Abdomen (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.TapeAbdomen,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.TapeAbdomen = value,
                Sex = 1
            };
            FormInputInfo tapeWaist = new()
            {
                Id = "bodyFatMeasureTapeWaist",
                Label = "Cintura (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.TapeWaist,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.TapeWaist = value,
                Sex = 0
            };
            FormInputInfo tapeHip = new()
            {
                Id = "bodyFatMeasureTapeHip",
                Label = "Cadera (mm)*",
                GetValue = () => physicicalData.BodyMeasure?.BodyFat?.Measures?.TapeHip,
                SetValue = value => physicicalData.BodyMeasure!.BodyFat!.Measures.TapeHip = value,
                Sex = 0
            };

            // Para los de un único sexo
            FormInputInfo tricepFem = tricep.GetCopy();
            tricepFem.Sex = 0;

            FormInputInfo suprailiacFem = suprailiac.GetCopy();
            suprailiacFem.Sex = 0;

            FormInputInfo chestMasc = chest.GetCopy();
            chestMasc.Sex = 1;

            FormInputInfo abdominalMasc = abdominal.GetCopy();
            abdominalMasc.Sex = 1;

            return new Dictionary<int, List<FormInputInfo>>
            {
                { 1, new List<FormInputInfo> { bicep, tricep, subscapular, suprailiac } },
                { 2, new List<FormInputInfo> { chest, abdominal, thigh, bicep, tricep, subscapular, suprailiac, lowerBack, calf } },
                { 31, new List<FormInputInfo> { midAxilary, chest, abdominal, thigh, suprailiac, subscapular, tricep } },
                { 32, new List<FormInputInfo> { abdominal, tricep, suprailiac, thigh } },
                { 33, new List<FormInputInfo> { chestMasc, abdominalMasc, tricepFem, suprailiacFem, thigh } },
                { 4, new List<FormInputInfo> { tapeNeck, tapeAbdomen, tapeWaist, tapeHip } }
            };
        }

        public static List<FormInputInfo> NewCircInputList(PhysicalData physicicalData)
        {
            FormInputInfo circLBicep = new()
            {
                Id = "bodyFatCircunferenceLeftBicep",
                Label = "Bícep izdo. (cm)",
                GetValue = () => physicicalData.BodyMeasure?.Circunferences?.LeftBicep ?? 0,
                SetValue = value => physicicalData.BodyMeasure!.Circunferences!.LeftBicep = (double)value!
            };
            FormInputInfo circRBicep = new()
            {
                Id = "bodyFatCircunferenceRightBicep",
                Label = "Bícep dcho. (cm)",
                GetValue = () => physicicalData.BodyMeasure?.Circunferences?.RightBicep ?? 0,
                SetValue = value => physicicalData.BodyMeasure!.Circunferences!.RightBicep = (double)value!
            };
            FormInputInfo circWaist = new()
            {
                Id = "bodyFatCircunferenceWaist",
                Label = "Cintura (cm)",
                GetValue = () => physicicalData.BodyMeasure?.Circunferences?.Waist ?? 0,
                SetValue = value => physicicalData.BodyMeasure!.Circunferences!.Waist = (double)value!
            };
            FormInputInfo circHip = new()
            {
                Id = "bodyFatCircunferenceHip",
                Label = "Cadera (cm)",
                GetValue = () => physicicalData.BodyMeasure?.Circunferences?.Hip ?? 0,
                SetValue = value => physicicalData.BodyMeasure!.Circunferences!.Hip = (double)value!
            };
            FormInputInfo circLThigh = new()
            {
                Id = "bodyFatCircunferenceLeftThigh",
                Label = "Muslo izdo. (cm)",
                GetValue = () => physicicalData.BodyMeasure?.Circunferences?.LeftThigh ?? 0,
                SetValue = value => physicicalData.BodyMeasure!.Circunferences!.LeftThigh = (double)value!
            };
            FormInputInfo circRThigh = new()
            {
                Id = "bodyFatCircunferenceRightThigh",
                Label = "Muslo dcho. (cm)",
                GetValue = () => physicicalData.BodyMeasure?.Circunferences?.RightThigh ?? 0,
                SetValue = value => physicicalData.BodyMeasure!.Circunferences!.RightThigh = (double)value!
            };

            return new List<FormInputInfo> { circLBicep, circRBicep, circWaist, circHip, circLThigh, circRThigh };
        }
        #endregion


        #region PhysicalDataListItem.razor
        public static List<Tuple<string, double>>? ProcessBodyFatMeasures(BodyFat? bodyFat)
        {
            if (bodyFat == null || bodyFat.Measures == null) return null;

            List<Tuple<string, double>> result = new();
            Dictionary<string, string> nameMapping = new()
            {
                {"Chest", "Pecho"},
                {"Abdominal", "Abdominal"},
                {"Thigh", "Muslo"},
                {"Tricep", "Tríceps"},
                {"Subscapular", "Subescapular"},
                {"Suprailiac", "Suprailíaco"},
                {"Midaxillary", "Axila"},
                {"Bicep", "Bícep"},
                {"LowerBack", "Espalda baja"},
                {"Calf", "Pantorrilla"},
                {"TapeNeck", "Cuello"},
                {"TapeAbdomen", "Abdomen"},
                {"TapeWaist", "Cintura"},
                {"TapeHip", "Cadera"}
            };

            foreach (var measureName in nameMapping.Keys)
            {
                var value = bodyFat.Measures.GetType().GetProperty(measureName)?.GetValue(bodyFat.Measures);

                if (value != null) result.Add(new Tuple<string, double>(nameMapping[measureName], (double)value));
            }
            return result;
        }

        public static List<Tuple<string, double>>? ProcessCircunferences(BodyMeasure bodyMeasure)
        {
            if (bodyMeasure.Circunferences == null) return null;

            List<Tuple<string, double>> result = new();
            Dictionary<string, string> nameMapping = new()
            {
                {"LeftBicep", "Bícep izdo."},
                {"RightBicep", "Bícep dcho."},
                {"Waist", "Cintura"},
                {"Hip", "Cadera"},
                {"LeftThigh", "Pierna izda."},
                {"RightThigh", "Pierna dcha."},
            };

            foreach (var measureName in nameMapping.Keys)
            {
                var value = bodyMeasure.Circunferences.GetType().GetProperty(measureName)?.GetValue(bodyMeasure.Circunferences);

                if (value != null) result.Add(new Tuple<string, double>(nameMapping[measureName], (double)value));
            }
            return result;
        }

        public static string ProcessBodyFatMethod(BodyFat bodyFat)
        {
            if (bodyFat?.Method != null)
            {
                switch (bodyFat!.Method)
                {
                    case 1: return "Durnin / Womersley";
                    case 2: return "Parrillo";
                    case 31: return "Jackson / Pollock - 7 medidas";
                    case 32: return "Jackson / Pollock - 4 medidas";
                    case 33: return "Jackson / Pollock - 3 medidas";
                    case 4: return "Cinta métrica";
                    default: break;
                }
            }
            return "-";
        }
        #endregion


        #region Index.razor
        public static string? GetFatPercSrc(double? lastFatPerc, int sex)
        {
            if (lastFatPerc == null) return null;

            string baseUrl = "./imgs/fatPercBodies/";
            string extension = ".webp";
            string sexBase = sex == 1 ? "male/" : "female/";
            string number = sex == 1 ? "m40" : "f45";
            List<Tuple<double, string>> fatPercMapping;

            if (sex == 1)
            {
                fatPercMapping = new()
                {
                    new Tuple<double, string>(0.08, "m8"),
                    new Tuple<double, string>(0.1, "m10"),
                    new Tuple<double, string>(0.15, "m15"),
                    new Tuple<double, string>(0.2, "m20"),
                    new Tuple<double, string>(0.25, "m25"),
                    new Tuple<double, string>(0.3, "m30"),
                    new Tuple<double, string>(0.35, "m35"),
                    new Tuple<double, string>(0.4, "m40")
                };
            }
            else
            {
                fatPercMapping = new()
                {
                    new Tuple<double, string>(0.12, "f12"),
                    new Tuple<double, string>(0.15, "f15"),
                    new Tuple<double, string>(0.2, "f20"),
                    new Tuple<double, string>(0.25, "f25"),
                    new Tuple<double, string>(0.3, "f30"),
                    new Tuple<double, string>(0.35, "f35"),
                    new Tuple<double, string>(0.4, "f40"),
                    new Tuple<double, string>(0.45, "f45")
                };
            }

            foreach (var mapping in fatPercMapping)
            {
                if (lastFatPerc < mapping.Item1)
                {
                    number = mapping.Item2;
                    break;
                }
            }

            return $"{baseUrl}{sexBase}{number}{extension}";
        }
        #endregion


        #region MainLayout.razor
        public static string ObjectIdToString(Dictionary<string, object> objectIdComponentsDict)
        {
            // Extraer y convertir cada componente a un entero
            int timestamp = ((JsonElement)objectIdComponentsDict["timestamp"]).GetInt32();
            int machine = ((JsonElement)objectIdComponentsDict["machine"]).GetInt32();
            int pid = ((JsonElement)objectIdComponentsDict["pid"]).GetInt32();
            int increment = ((JsonElement)objectIdComponentsDict["increment"]).GetInt32();

            // Convertir cada entero a una cadena hexadecimal y rellenar con ceros a la izquierda si es necesario
            string timestampHex = timestamp.ToString("x8");
            string machineHex = machine.ToString("x6");
            string pidHex = pid.ToString("x4");
            string incrementHex = increment.ToString("x6");

            // Concatena los componentes para obtener la cadena ObjectId
            return timestampHex + machineHex + pidHex + incrementHex;
        }
        #endregion


        #region Food.razor y otros
        public static int CreateDayTimeIntakeType(List<FoodData>? foodDataList, List<MealIntake>? mealDataList)
        {
            bool hasFoodData = foodDataList?.Count > 0;
            bool hasMealData = mealDataList?.Count > 0;

            if (hasFoodData && !hasMealData) { return 0; }
            else if (!hasFoodData && hasMealData) { return 1; }
            else if (hasFoodData && hasMealData) { return 2; }
            return 0;
        }
        #endregion


        #region Status utilities
        public static HistoricalDataStatus GetHistoricalDataStatus(HistoricalData histData)
        {
            return new HistoricalDataStatus
            {
                IsPhysicalData = histData.PhysicalData != null,
                IsNutritionalData = histData.NutritionalData != null,
            };
        }

        public static NutritionalDataStatus GetNutritionalDataStatus(NutritionalData nutData)
        {
            return new NutritionalDataStatus
            {
                IsAllDayData = nutData.DayTimeIntakes == null,
                DayTimeIntakeDictStatus = (nutData.DayTimeIntakes != null) ? GetIntakeDictStatus(nutData.DayTimeIntakes) : null
            };
        }

        public static DayTimeIntakeDictStatus GetIntakeDictStatus(Dictionary<int, DayTimeIntakes> dayTimeIntakesDict)
        {
            Dictionary<int, DayTimeIntakeStatus> tempDayTimeStatusesDict = new();
            foreach ((int key, DayTimeIntakes dayTimeIntakes) in dayTimeIntakesDict)
            {
                tempDayTimeStatusesDict.Add(key, GetIntakeDayStatus(dayTimeIntakes));
            }

            return new DayTimeIntakeDictStatus
            {
                LastDayTimeKey = dayTimeIntakesDict.Keys.Count() == 1,
                DayTimeIntakeStatus = tempDayTimeStatusesDict
            };
        }

        public static DayTimeIntakeStatus GetIntakeDayStatus(DayTimeIntakes dayTimeIntakes)
        {
            return new DayTimeIntakeStatus
            {
                LastFoodIntake = (dayTimeIntakes.FoodIntake == null) ? null : dayTimeIntakes.FoodIntake.Count == 1,
                LastMealIntake = (dayTimeIntakes.MealIntake == null) ? null : dayTimeIntakes.MealIntake.Count == 1
            };
        }
        #endregion

        public static DeleteStructureData? ConditionalDeleteInfo(int dayTime, bool isMeal, NutritionalData nutData, string? mealId = null, string? foodId = null)
        {
            DeleteStructureData processedDeleteStructureData = new();

            DayTimeIntakeStatus dayTimeIntakeStatus = GetIntakeDayStatus(nutData.DayTimeIntakes![dayTime]);
            DayTimeIntakes dayTimeData = nutData.DayTimeIntakes![dayTime];

            if (isMeal && dayTimeData.MealIntake?.FirstOrDefault(x => x.MealId == mealId) == null) return null;
            if (!isMeal && dayTimeData.FoodIntake?.FirstOrDefault(x => x.Id == foodId) == null) return null;

            bool isLastMainIntake = isMeal ? dayTimeIntakeStatus.LastMealIntake == true : dayTimeIntakeStatus.LastFoodIntake == true;
            bool noSecondaryIntake = isMeal ? !dayTimeIntakeStatus.LastFoodIntake ?? true : !dayTimeIntakeStatus.LastMealIntake ?? true;


            // Si es el último Intake (meal o food) y no hay otro tipo de intake (food o meal, respectivamente)
            if (isLastMainIntake && noSecondaryIntake)
            {
                processedDeleteStructureData.DeleteDayTimePair = true;
            }
            else if (isLastMainIntake)
            {
                if (isMeal) processedDeleteStructureData.DeleteMealIntakeList = true;
                else processedDeleteStructureData.DeleteFoodIntakeList = true;

                processedDeleteStructureData.UpdateDayTimeIntakeType = true;
            }
            else
            {
                if (isMeal)
                {
                    processedDeleteStructureData.DeleteMealIntakeItem = true;
                    processedDeleteStructureData.IntakeIndexToDelete = dayTimeData.MealIntake!.IndexOf(dayTimeData.MealIntake!.FirstOrDefault(x => x.MealId == mealId)!);
                }
                else
                {
                    processedDeleteStructureData.DeleteFoodIntakeItem = true;
                    processedDeleteStructureData.IntakeIndexToDelete = dayTimeData.FoodIntake!.IndexOf(dayTimeData.FoodIntake!.FirstOrDefault(x => x.Id == foodId)!);
                }
                processedDeleteStructureData.UpdateDayTimeIntakeCalMacros = true;
            }

            return processedDeleteStructureData;
        }

        #region DailyIntakeMeal&FoodDetail
        public static Dictionary<int, DayTimeIntakesDTO> DayTimeIntakesDictTODTODict(Dictionary<int, DayTimeIntakes> dayTimeIntakesDict)
        {
            Dictionary<int, DayTimeIntakesDTO> tempDTIDTO = new();

            foreach ((int dayTime, DayTimeIntakes intakes) in dayTimeIntakesDict)
            {
                List<string>? foodIntakes = null;
                if (intakes.FoodIntake != null)
                {
                    foodIntakes = intakes.FoodIntake!.Select(x => x.Id!).ToList();
                }
                tempDTIDTO.Add(
                    dayTime,
                    new()
                    {
                        Type = intakes.Type,
                        FoodIntake = foodIntakes,
                        MealIntake = intakes.MealIntake,
                        TotalDayTimeIntakesCalories = intakes.TotalDayTimeIntakesCalories,
                        TotalDayTimeIntakesMacros = intakes.TotalDayTimeIntakesMacros
                    }
                );
            }

            return tempDTIDTO;
        }

        #endregion
    }
}
