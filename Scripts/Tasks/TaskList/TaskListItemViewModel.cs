using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Media;
using DailyProject_221204.Scripts;

namespace DailyProject_221204
{
    public class TaskListItemViewModel : AbstractViewModel<TaskModel>
    {        readonly ViewColor _normalColor = new("#FF2D2D2D");
        readonly ViewColor _highlightColor = new("#FFCECECE");

        public TaskListItemViewModel(TaskModel model) : base(model)
        {
        }

        public EventCommand RemoveEventCommand { get; } = new();
        public EventCommand SelectEventCommand { get; } = new();
        public EventCommand ChangeStatusEventCommand { get; } = new();
        public EventCommand AddScheduleEventCommand { get; } = new();

        public SolidColorBrush BackgroundColor { get; private set; } = new SolidColorBrush();

        public void TurnOffHighlight()
        {
            BackgroundColor = _normalColor.Color;
            _onPropertyChanged(nameof(BackgroundColor));
        }
        public void TurnOnHighlight()
        {
            BackgroundColor = _highlightColor.Color;
            _onPropertyChanged(nameof(BackgroundColor));
        }
    }
}
