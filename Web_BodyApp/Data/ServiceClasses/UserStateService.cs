using System.Collections.ObjectModel;
using System.ComponentModel;
using Web_BodyApp.Data.DTOs;
using Web_BodyApp.Data.Models;

namespace Web_BodyApp.Data.ServiceClasses
{
    public class UserStateService : INotifyPropertyChanged
    {
        private UserDTO userData;
        private ObservableCollection<HistoricalData> historicalData = new();
        private ObservableCollection<Meal> userCustomMeals = new();


        public UserDTO UserData
        {
            get { return userData; }
            set
            {
                if (userData != value)
                {
                    userData = value;
                    OnPropertyChanged(nameof(UserData));
                }
            }
        }
        public ObservableCollection<HistoricalData> HistoricalData
        {
            get { return historicalData; }
            set
            {
                if (historicalData != value)
                {
                    historicalData = new ObservableCollection<HistoricalData>(value);
                    OnPropertyChanged(nameof(HistoricalData));
                }
            }
        }
        public ObservableCollection<Meal> Meals
        {
            get { return userCustomMeals; }
            set
            {
                if (userCustomMeals != value)
                {
                    userCustomMeals = new ObservableCollection<Meal>(value);
                    OnPropertyChanged(nameof(Meals));
                }
            }
        }
        public List<string> UnorderedHistoricalDataIds { get; set; } = new();


        public event PropertyChangedEventHandler PropertyChanged;

        public List<string> GetHistoricalDataStringList()
        {
            return HistoricalData.Select(historicalData => historicalData.Id!).ToList();
        }

        public List<string> GetMealsStringList()
        {
            return Meals.Select(meal => meal.Id!).ToList();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
