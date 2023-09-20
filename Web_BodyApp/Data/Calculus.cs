using Web_BodyApp.Data.DTOs;
using Web_BodyApp.Data.Models;
using Web_BodyApp.Data.Models.NutritionData;
using Web_BodyApp.Data.Models.PhysicData;

namespace Web_BodyApp.Data
{
    public class Calculus
    {
        public static double ImcCalc(double height, double weight)
        {
            return weight / Math.Pow(height / 100, 2);
        }

        public static WeightComposition WeightCompositionCalc(int sex, double weight, double height, double fatPercentage)
        {
            // TODO Hacer estos valores dependientes de la BDD - si no están, se usarán esos como valores por defecto
            double biestiloideusWristDiameter = 0.055; // Diámetro biestiloideo de la muñeca (m)
            double biepicondilarFemurDiameter = 0.097; // Diámetro biepicondilar del fémur (m)

            var fatWeight = weight * fatPercentage;
            var residualWeight = weight * (sex == 1 ? 0.241 : 0.209);
            var boneWeight = 3.02 *
                Math.Pow(
                    (Math.Pow(height / 100, 2) * biepicondilarFemurDiameter * biestiloideusWristDiameter * 400), 0.712);
            var muscleWeight = weight - (fatWeight + residualWeight + boneWeight);

            return new WeightComposition
            {
                BoneWeight = boneWeight,
                FatWeight = fatWeight,
                ResidualWeight = residualWeight,
                MuscleWeight = muscleWeight
            };
        }

        public static double? FatPercentageCalc(UserDTO user, double weight, BodyFat bodyFatMeasures)
        {
            // Sex:
            // 0 - mujer;
            // 1 - hombre

            // Method:
            // 1 - Durnin / Womersley;
            // 2 - Parrillo;
            // 31 - Jackson/Pollock 7 medidas;
            // 32 - Jackson/Pollock 4 medidas;
            // 33 - Jackson/Pollock 3 medidas;
            // 4 - Cinta métrica

            double sumAllMeasures;
            switch (bodyFatMeasures.Method)
            {
                case 1:
                    sumAllMeasures =
                        (double)bodyFatMeasures.Measures.Tricep! +
                        (double)bodyFatMeasures.Measures.Subscapular! +
                        (double)bodyFatMeasures.Measures.Suprailiac! +
                        (double)bodyFatMeasures.Measures.Bicep!;
                    double logSumAllMeasures = Math.Log10(sumAllMeasures);

                    double bodyDensity;
                    if (user.Age < 17)
                    {
                        bodyDensity = (user.Sex == 1) ? 1.1533 - (0.0643 * logSumAllMeasures) : 1.1369 - (0.0598 * logSumAllMeasures);
                    }
                    else if (user.Age >= 17 && user.Age <= 19)
                    {
                        bodyDensity = (user.Sex == 1) ? 1.1620 - (0.0630 * logSumAllMeasures) : 1.1549 - (0.0678 * logSumAllMeasures);
                    }
                    else if (user.Age >= 20 && user.Age <= 29)
                    {
                        bodyDensity = (user.Sex == 1) ? 1.1631 - (0.0632 * logSumAllMeasures) : 1.1599 - (0.0717 * logSumAllMeasures);
                    }
                    else if (user.Age >= 30 && user.Age <= 39)
                    {
                        bodyDensity = (user.Sex == 1) ? 1.1422 - (0.0544 * logSumAllMeasures) : 1.1423 - (0.0632 * logSumAllMeasures);
                    }
                    else if (user.Age >= 40 && user.Age <= 49)
                    {
                        bodyDensity = (user.Sex == 1) ? 1.1620 - (0.0700 * logSumAllMeasures) : 1.1333 - (0.0612 * logSumAllMeasures);
                    }
                    else
                    {
                        bodyDensity = (user.Sex == 1) ? 1.1715 - (0.0779 * logSumAllMeasures) : 1.1339 - (0.0645 * logSumAllMeasures);
                    }
                    return ((495 / bodyDensity) - 450) / 100;

                case 2:
                    sumAllMeasures =
                        (double)bodyFatMeasures.Measures.Chest! +
                        (double)bodyFatMeasures.Measures.Abdominal! +
                        (double)bodyFatMeasures.Measures.Thigh! +
                        (double)bodyFatMeasures.Measures.Bicep! +
                        (double)bodyFatMeasures.Measures.Tricep! +
                        (double)bodyFatMeasures.Measures.Subscapular! +
                        (double)bodyFatMeasures.Measures.Suprailiac! +
                        (double)bodyFatMeasures.Measures.LowerBack! +
                        (double)bodyFatMeasures.Measures.Calf!;
                    double weightInPounds = weight * 2.20462;
                    return (sumAllMeasures * 27) / weightInPounds;

                case 31:
                    sumAllMeasures =
                        (double)bodyFatMeasures.Measures.Chest! +
                        (double)bodyFatMeasures.Measures.Abdominal! +
                        (double)bodyFatMeasures.Measures.Thigh! +
                        (double)bodyFatMeasures.Measures.Tricep! +
                        (double)bodyFatMeasures.Measures.Subscapular! +
                        (double)bodyFatMeasures.Measures.Suprailiac! +
                        (double)bodyFatMeasures.Measures.MidAxilary!;

                    if (user.Sex == 1)
                    {
                        bodyDensity = 1.112 - (0.00043499 * sumAllMeasures) + (0.00000055 * Math.Pow(sumAllMeasures, 2)) - (0.00028826 * user.Age);
                    }
                    else
                    {
                        bodyDensity = 1.097 - (0.00046971 * sumAllMeasures) + (0.00000056 * Math.Pow(sumAllMeasures, 2)) - (0.00012828 * user.Age);
                    }
                    return ((495 / bodyDensity) - 450) / 100;

                case 32:
                    sumAllMeasures =
                        (double)bodyFatMeasures.Measures.Suprailiac! +
                        (double)bodyFatMeasures.Measures.Abdominal! +
                        (double)bodyFatMeasures.Measures.Thigh! +
                        (double)bodyFatMeasures.Measures.Tricep!;

                    if (user.Sex == 1)
                    {
                        return (0.29288 * sumAllMeasures) - (0.0005 * Math.Pow(sumAllMeasures, 2)) + (0.15845 * user.Age) - 5.76377;
                    }
                    else
                    {
                        return (0.29669 * sumAllMeasures) - (0.00043 * Math.Pow(sumAllMeasures, 2)) + (0.02963 * user.Age) + 1.4072;
                    }

                case 33:

                    if (user.Sex == 1)
                    {
                        double sumAllMeasuresMan =
                            (double)bodyFatMeasures.Measures.Abdominal! +
                            (double)bodyFatMeasures.Measures.Thigh! +
                            (double)bodyFatMeasures.Measures.Chest!;
                        bodyDensity = 1.10938 - (0.0008267 * sumAllMeasuresMan) + (0.0000016 * Math.Pow(sumAllMeasuresMan, 2)) - (0.0002574 * user.Age);
                    }
                    else
                    {
                        double sumAllMeasuresWoman =
                            (double)bodyFatMeasures.Measures.Tricep! +
                            (double)bodyFatMeasures.Measures.Thigh! +
                            (double)bodyFatMeasures.Measures.Suprailiac!;
                        bodyDensity = 1.0994921 - (0.0009929 * sumAllMeasuresWoman) + (0.0000023 * Math.Pow(sumAllMeasuresWoman, 2)) - (0.0001392 * user.Age);
                    }
                    return ((495 / bodyDensity) - 450) / 100;

                case 4:
                    if (user.Sex == 1)
                    {
                        double calc =
                            (double)bodyFatMeasures.Measures.TapeAbdomen! -
                            (double)bodyFatMeasures.Measures.TapeNeck!;

                        double result = 495 / (1.0324 - 0.19077 * Math.Log10(calc) + 0.15456 * Math.Log10(user.Height)) - 450;
                        return Math.Abs(result) / 100;
                    }
                    else
                    {
                        double calc =
                            (double)bodyFatMeasures.Measures.TapeWaist! +
                            (double)bodyFatMeasures.Measures.TapeHip! -
                            (double)bodyFatMeasures.Measures.TapeNeck!;

                        double result = 495 / (1.29579 - 0.35004 * Math.Log10(calc) + 0.22100 * Math.Log10(user.Height)) - 450;
                        return Math.Abs(result) / 100;
                    }

                default:
                    return null;
            }
        }

        public static double MusclePercentageCalc(double weight, double muscleWeight)
        {
            return (muscleWeight / weight);
        }

        public static Nutrients MacrosObjetivo(UserDTO usuario, double peso)
        {
            double tmbCals;
            double maintenanceCals = 0.0;
            double weightLossCals = 0.0;

            // Para TMB
            if (usuario.Sex == 0)
            {
                tmbCals = (double)(665.51 + (9.463 * peso) + (1.8 * usuario.Height) - (4.6756 * usuario.Age));
            }
            else
            {
                tmbCals = (double)(66.473 + (13.751 * peso) + (5.0033 * usuario.Height) - (6.55 * usuario.Age));
            }

            // Para calorias de mantenimiento
            if (usuario.ActivityLevel == 0)
            {
                maintenanceCals = tmbCals * 1.375;
            }
            else if (usuario.ActivityLevel == 1)
            {
                maintenanceCals = tmbCals * 1.55;
            }
            else if (usuario.ActivityLevel == 2)
            {
                maintenanceCals = tmbCals * 1.725;
            }

            // Para calorías de pérdida de peso
            if (usuario.FatLooseRate == 0)
            {
                weightLossCals = maintenanceCals - (maintenanceCals * 0.1);
            }
            else if (usuario.FatLooseRate == 1)
            {
                weightLossCals = maintenanceCals - (maintenanceCals * 0.18);
            }
            else if (usuario.FatLooseRate == 2)
            {
                weightLossCals = maintenanceCals - (maintenanceCals * 0.25);
            }

            return new Nutrients
            {
                DiaryTMB = (int)Math.Round(tmbCals),
                MaintainKcals = (int)Math.Round(maintenanceCals),
                FatLossKcals = (int)Math.Round(weightLossCals),
                Macros = DailyMacrosObjective(
                    usuario,
                    (int)Math.Round(maintenanceCals),
                    (int)Math.Round(weightLossCals)
                )
            };
        }

        private static Macros DailyMacrosObjective(UserDTO usuario, int maintainCals, int fatLooseCals)
        {
            double percProts = 0.3;
            double percCarbs;
            double percFats;
            int gramsProts;
            int gramsCarbs;
            int gramsFats;

            // Porcentajes de macros
            if (usuario.ActivityLevel == 0)
            {
                percCarbs = 0.4;
                percFats = 0.3;
            }
            else
            {
                percCarbs = 0.35;
                percFats = 0.35;
            }

            // Gramos de macros
            int referenceCals = usuario.Goal == 0 ? maintainCals : fatLooseCals;

            gramsProts = (int)((referenceCals * percProts) / 4);
            gramsCarbs = (int)((referenceCals * percCarbs) / 4);
            gramsFats = (int)((referenceCals * percFats) / 9);

            return new Macros
            {
                PercProt = percProts,
                PercCarbs = percCarbs,
                PercFat = percFats,
                GramsProt = gramsProts,
                GramsCarbs = gramsCarbs,
                GramsFat = gramsFats
            };
        }


        public static (int, FoodMacros) CalculateMealData(List<Meal> mealsList, List<FoodServing> mealsServingList)
        {
            int totalCals = 0;
            FoodMacros totalMacros = new FoodMacros();

            int internalTempMealDataIdx = 0;
            foreach (Meal internalTempMealData in mealsList)
            {
                int qtyMultiplier = 0;
                if (mealsServingList![internalTempMealDataIdx].ServingUnit == 0)
                {
                    qtyMultiplier = internalTempMealData.TotalWeight * ((int)mealsServingList[internalTempMealDataIdx].ServingQty / 100);
                }
                else
                {
                    qtyMultiplier = internalTempMealData.TotalWeight / (int)mealsServingList[internalTempMealDataIdx].ServingQty;
                }

                totalCals += internalTempMealData.TotalCalories * qtyMultiplier;
                totalMacros.Carbs += internalTempMealData.TotalMealMacros.Carbs * qtyMultiplier;
                totalMacros.Prots += internalTempMealData.TotalMealMacros.Prots * qtyMultiplier;
                totalMacros.Fats += internalTempMealData.TotalMealMacros.Fats * qtyMultiplier;

                internalTempMealDataIdx++;
            }

            return (totalCals, totalMacros);
        }

        public static double CalculateMealServingMultiplier(Meal meal, FoodServing serving)
        {
            return serving.ServingUnit == 0
                ? serving.ServingQty / 100
                : serving.ServingQty / meal.TotalWeight;
        }


        public static (int, FoodMacros) CalculateNutDataCalsMacros(NutritionalData nutData, List<Meal> userMeals)
        {
            int totalCals = 0;
            FoodMacros totalMacros = new FoodMacros();

            if (nutData.DayTimeIntakes == null)
            {
                totalCals = nutData.TotalNutDataCalories;
                totalMacros = nutData.TotalNutDataMacros;
            }
            else
            {
                foreach (DayTimeIntakes intakeInfo in nutData.DayTimeIntakes.Values)
                {
                    var (intakeInfoTotalCals, intakeInfoTotalMacros) = CalculateDayTimeIntakesCalsMacros(intakeInfo, userMeals);

                    totalCals += intakeInfoTotalCals;
                    totalMacros.Carbs += intakeInfoTotalMacros.Carbs;
                    totalMacros.Prots += intakeInfoTotalMacros.Prots;
                    totalMacros.Fats += intakeInfoTotalMacros.Fats;
                }
            }

            return (totalCals, totalMacros);
        }

        public static (int, FoodMacros) CalculateDayTimesIntakesCompleteDictCalsMacros(Dictionary<int, DayTimeIntakes> dayTimeIntakesDict, List<Meal> userMeals)
        {
            int totalCals = 0;
            FoodMacros totalMacros = new FoodMacros();

            foreach (DayTimeIntakes intakeInfo in dayTimeIntakesDict.Values)
            {
                var (intakeInfoTotalCals, intakeInfoTotalMacros) = CalculateDayTimeIntakesCalsMacros(intakeInfo, userMeals);

                totalCals += intakeInfoTotalCals;
                totalMacros.Carbs += intakeInfoTotalMacros.Carbs;
                totalMacros.Prots += intakeInfoTotalMacros.Prots;
                totalMacros.Fats += intakeInfoTotalMacros.Fats;
            }

            return (totalCals, totalMacros);
        }

        public static (int, FoodMacros) CalculateDayTimeIntakesCalsMacros(DayTimeIntakes dayTimeIntakes, List<Meal> userMeals)
        {
            int totalDayTimeCals = 0;
            FoodMacros totalDayTimeMacros = new();

            if (dayTimeIntakes.FoodIntake?.Count > 0)
            {
                foreach (FoodData fData in dayTimeIntakes.FoodIntake)
                {
                    totalDayTimeCals += fData.FoodCalories;

                    totalDayTimeMacros.Carbs += fData.FoodMacros.Carbs;
                    totalDayTimeMacros.Prots += fData.FoodMacros.Prots;
                    totalDayTimeMacros.Fats += fData.FoodMacros.Fats;
                }
            }

            if (dayTimeIntakes.MealIntake?.Count > 0)
            {
                foreach (MealIntake mIntake in dayTimeIntakes.MealIntake)
                {
                    Meal meal = userMeals.Find(m => m.Id == mIntake.MealId)!;
                    double mealMultiplier = CalculateMealServingMultiplier(meal, mIntake.MealServing);
                    var (mealCals, mealMacros) = CalculateMealIntakeMacros(mIntake, meal);

                    totalDayTimeCals += mealCals;
                    totalDayTimeMacros.Carbs += mealMacros.Carbs;
                    totalDayTimeMacros.Prots += mealMacros.Prots;
                    totalDayTimeMacros.Fats += mealMacros.Fats;
                }
            }

            return (totalDayTimeCals, totalDayTimeMacros);
        }

        public static (int, FoodMacros) CalculateMealIntakeMacros(MealIntake mealIntake, Meal usedMeal)
        {
            double mealMultiplier = CalculateMealServingMultiplier(usedMeal, mealIntake.MealServing);

            return (
                (int)(usedMeal.TotalCalories * mealMultiplier),
                new FoodMacros()
                {
                    Carbs = (int)(usedMeal.TotalMealMacros.Carbs * mealMultiplier),
                    Prots = (int)(usedMeal.TotalMealMacros.Prots * mealMultiplier),
                    Fats = (int)(usedMeal.TotalMealMacros.Fats * mealMultiplier)
                }
            );
        }
    }
}
