﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Clients;
using Api.Exceptions;
using GalaSoft.MvvmLight.Command;
using Models;
using Models.Validate;

namespace EvotoClient.ViewModel
{
    public class RegisterViewModel : EvotoViewModelBase
    {
        private readonly UserClient _userClient;
        private readonly RegisterModelValidator _validator;

        public RegisterViewModel()
        {
            _validator = new RegisterModelValidator();
            _userClient = new UserClient();
            RegisterCommand = new RelayCommand<object>(Register);
            ReturnToLoginCommand = new RelayCommand(BackToLogin);
        }

        #region Commands

        public RelayCommand<object> RegisterCommand { get; }
        public RelayCommand ReturnToLoginCommand { get; }

        #endregion

        #region Properties

        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set
            {
                Set(ref _loading, value);
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { Set(ref _errorMessage, value); }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                Set(ref _firstName, value);
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                Set(ref _lastName, value);
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                Set(ref _email, value);
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        private string _idNumber;
        public string IdNumber
        {
            get { return _idNumber; }
            set
            {
                Set(ref _idNumber, value);
                RegisterCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Methods

        private bool IsFormValid(object parameter, bool updateErrorMessage, out RegisterModel registerModel)
        {
            registerModel = null;
            var valid = true;
            var errorMessages = new List<string>();

            var passwordContainer = parameter as IHavePasswords;
            if (passwordContainer == null)
                return false;

            var p1 = LoginViewModel.ConvertToUnsecureString(passwordContainer.SecurePassword);
            var p2 = LoginViewModel.ConvertToUnsecureString(passwordContainer.SecurePasswordConfirm);

            if (p1 != p2)
            {
                errorMessages.Add("Passwords do not match");
                valid = false;
                if (!updateErrorMessage)
                    return false;
            }

            registerModel = new RegisterModel(Email, FirstName, LastName, IdNumber, p1, p2);
            var v = _validator.Validate(registerModel);
            if (!v.IsValid)
            {
                errorMessages.AddRange(v.Errors.Select(e => e.ErrorMessage));
                valid = false;
            }

            if (updateErrorMessage && !valid)
                ErrorMessage = string.Join("\n", errorMessages);
            return valid;
        }

        private void Register(object parameter)
        {
            RegisterModel registerModel;
            if (!IsFormValid(parameter, true, out registerModel))
                return;

            Loading = true;
            ErrorMessage = "";
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await _userClient.Register(registerModel);
                    MainVm.ChangeView(EvotoView.Home);
                }
                catch (BadRequestException e)
                {
                    Ui(() =>
                    {
                        ErrorMessage = e.Message;
                        Loading = false;
                    });
                }
                catch (ApiException)
                {
                    Ui(() =>
                    {
                        ErrorMessage = "An Unknown Error Occurred";
                        Loading = false;
                    });
                }
            });
        }

        private void BackToLogin()
        {
            MainVm.ChangeView(EvotoView.Login);
        }

        #endregion
    }
}