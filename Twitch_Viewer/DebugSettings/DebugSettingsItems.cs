using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Twitch_Viewer
{
    public abstract class DebugSettingsItem : INotifyPropertyChanged
    {
        public string Description { get; set; }

        public Visibility IsCheckBoxVisible { get; set; } = Visibility.Collapsed;
        public Visibility IsTextBoxVisible { get; set; } = Visibility.Collapsed;
        public Visibility IsValueBoxVisible { get; set; } = Visibility.Collapsed;
        public Visibility IsButtonVisible { get; set; } = Visibility.Collapsed;
        public Visibility IsSeparatorVisible { get; set; } = Visibility.Collapsed;

        public event PropertyChangedEventHandler PropertyChanged;

        protected DebugSettingsItem(string settingName, string description)
        {
            Description = description;
            if (this is DebugSettingCheckBox || this is DebugSettingTextBox || this is DebugSettingButtonWithValue)
                MainWindow.settings.DebugSettings.Add(settingName, this);
        }

        protected DebugSettingsItem(string description)
        {
            Description = description;
        }

        protected DebugSettingsItem()
        {
        }

        protected void OnPropertyChanged(MethodBase methodBase)
        {
            OnPropertyChanged(methodBase.Name.Substring(4));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
        private string _textBoxText = "";
        public string TextBoxText { get { return _textBoxText; } set { _textBoxText = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); } }

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

        public delegate void buttonActionDelegate(params object[] parameters);

        public DebugSettingButton(string description, string buttonText, buttonActionDelegate action, params object[] actionParameters)
            : base(description)
        {
            _buttonAction = action;
            _buttonActionParameters = actionParameters;

            IsButtonVisible = Visibility.Visible;
            ButtonText = buttonText;
        }

        protected DebugSettingButton(string settingName, string description, string buttonText, buttonActionDelegate action, params object[] actionParameters)
            : base(settingName, description)
        {
            _buttonAction = action;
            _buttonActionParameters = actionParameters;

            IsButtonVisible = Visibility.Visible;
            ButtonText = buttonText;
        }
    }

    public class DebugSettingButtonWithValue : DebugSettingButton
    {
        private string _value = "";
        public string Value { get { return _value; } set { _value = value; OnPropertyChanged(MethodBase.GetCurrentMethod()); } }

        public DebugSettingButtonWithValue(string settingName, string description, string buttonText, buttonActionDelegate action, string startValue = "", params object[] actionParameters)
            : base(settingName, description, buttonText, action, actionParameters)
        {
            _value = startValue;
            IsValueBoxVisible = Visibility.Visible;
        }
    }

    public class DebugSettingSeparator : DebugSettingsItem
    {
        public DebugSettingSeparator()
            : base()
        {
            IsSeparatorVisible = Visibility.Visible;
        }
    }
}
