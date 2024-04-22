using System;

namespace password_manager
{
    public partial class LoginActivity
    {
        public class AuthenticationErrorEventArgs : EventArgs
        {
            public string ErrorMessage { get; }

            public AuthenticationErrorEventArgs(string errorMessage)
            {
                ErrorMessage = errorMessage;
            }
        }
    }
}