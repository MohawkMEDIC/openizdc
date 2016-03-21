
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Mobile.Core.Android;
using OpenIZ.Mobile.Core.Services;
using System.Security;

namespace OpenIZMobile
{
	[Activity (Label = "@string/activity_login", NoHistory = true, Theme = "@style/OpenIZ.NoTitle")]
	public class LoginActivity : Activity
	{

		// Username / password controls
		private EditText m_txtUserName; 
		private EditText m_txtPassword; 
		private Button m_btnLogin;

		protected override void OnCreate (Bundle savedInstanceState)
		{

			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Login);
			// grab controls
			this.m_txtUserName = this.FindViewById<EditText>(Resource.Id.txt_login_username);
			this.m_txtPassword = this.FindViewById<EditText>(Resource.Id.txt_login_password);
			this.m_btnLogin = this.FindViewById<Button> (Resource.Id.btn_login_login);

			this.m_txtUserName.Text = "administrator";
			this.m_txtPassword.Text = "password";
			// Wire up events
			this.m_btnLogin.Click += (o,e)=>
			{

				// Get the identity service and attempt a login
				try
				{
					AndroidApplicationContext.Current.Authenticate(this.m_txtUserName.Text, this.m_txtPassword.Text);
					Intent mainPageIntent = new Intent(ApplicationContext, typeof(HomeActivity));
					this.StartActivity(mainPageIntent);
				}
				catch(Exception ex)
				{

					// Get the message for the error
					int msgId = Resources.GetIdentifier(ex.Message, "string", this.PackageName);
					if(msgId != 0)
						UserInterfaceUtils.ShowMessage(this, null, "{0} : {1}", Resources.GetString(Resource.String.err_login), Resources.GetString(msgId));
					else
						UserInterfaceUtils.ShowMessage(this, null, "{0} : {1}", Resources.GetString(Resource.String.err_login), ex.Message);

				}
			};

		}
	}
}

