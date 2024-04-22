using System;
using AndroidX.Biometric;
using Java.Lang;

namespace password_manager
{
    public partial class LoginActivity
    {
        class AuthenticationCallback : BiometricPrompt.AuthenticationCallback
        {
            public event EventHandler AuthenticationSucceeded;
            public event EventHandler<AuthenticationErrorEventArgs> AuthenticationError;
            public event EventHandler AuthenticationFailed;

            public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult result)
            {
                AuthenticationSucceeded?.Invoke(this, EventArgs.Empty);
            }

            public override void OnAuthenticationError(int errorCode, ICharSequence errString)
            {
                AuthenticationError?.Invoke(this, new AuthenticationErrorEventArgs(errString.ToString()));
            }

            public override void OnAuthenticationFailed()
            {
                AuthenticationFailed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}