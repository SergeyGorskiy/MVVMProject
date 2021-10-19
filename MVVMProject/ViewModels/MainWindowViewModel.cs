using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MVVMProject.Infrastructure.Commands;
using MVVMProject.Models;
using MVVMProject.ViewModels.Base;

namespace MVVMProject.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private IEnumerable<DataPoint> _testDataPoints;
        public IEnumerable<DataPoint> TestDataPoints
        {
            get => _testDataPoints;
            set => Set(ref _testDataPoints, value);
        }


        private string _title = "Анализ статистики";
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
        private string _status = "Готов!";
        public string Status
        {
            get => _status;
            set => Set(ref _title, value);
        }

        #region Команды

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }
        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        private bool CanCloseApplicationCommandExecute(object p) => true;

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new LambdaCommand
            (OnCloseApplicationCommandExecuted, 
                CanCloseApplicationCommandExecute);

            #endregion


            var dataPoints = new List<DataPoint>((int)(360 / 0.1));
            for (var x = 0d; x <= 360; x += 0.1)
            {
                const double toRed = Math.PI / 180;
                var y = Math.Sin(x * toRed);
                dataPoints.Add(new DataPoint{ XValue = x, YValue = y});
            }

            TestDataPoints = dataPoints;
        }
    }
}