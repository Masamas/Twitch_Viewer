using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Twitch_Viewer
{
    public abstract class DebugSettingsItem
    {
        public string Description { get; set; }

        public Visibility IsCheckBoxVisible { get; set; } = Visibility.Collapsed;
        public Visibility IsTextBoxVisible { get; set; } = Visibility.Collapsed;
        public Visibility IsButtonVisible { get; set; } = Visibility.Collapsed;

        protected DebugSettingsItem(string settingName, string description)
        {
            Description = description;
            if (this is DebugSettingCheckBox || this is DebugSettingTextBox)
                MainWindow.settings.DebugSettings.Add(settingName, this);
        }

        protected DebugSettingsItem(string description)
        {
            Description = description;
        }
    }

    public class DebugSettingCheckBox : DebugSettingsItem
    {
        public bool CheckBoxChecked { get; set; } = false;

        public DebugSettingCheckBox(string settingName, string description, bool isChecked = false)
            : base(settingName, description)
        {
            CheckBoxChecked = isChecked;
            IsCheckBoxVisible = Visibility.Visible;
        }
    }

    public class DebugSettingTextBox : DebugSettingsItem
    {
        public string TextBoxText { get; set; } = "";

        public DebugSettingTextBox(string settingName, string description)
            : base(settingName, description)
        {
            IsTextBoxVisible = Visibility.Visible;
        }
    }

    public class DebugSettingButton : DebugSettingsItem
    {
        private ICommand _buttonActionCommand;
        private buttonActionDelegate _buttonAction;
        private object _buttonActionParameters;

        public ICommand ButtonActionCommand
        {
            get
            {
                if (_buttonActionCommand == null)
                {
                    _buttonActionCommand = new RelayCommand(
                        param => this._buttonAction(_buttonActionParameters)
                    );
                }
                return _buttonActionCommand;
            }
        }

        public string ButtonText { get; }

        public delegate void buttonActionDelegate(object parameters);

        public DebugSettingButton(string description, string buttonText, buttonActionDelegate action, object actionParameters)
            : base(description)
        {
            _buttonAction = action;
            _buttonActionParameters = actionParameters;

            IsButtonVisible = Visibility.Visible;
            ButtonText = buttonText;
        }
    }

    public enum DebugSettingsKind
    {
        CheckBox,
        TextBox,
        Button
    }
}
