
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

namespace OpenIZMobile
{
	[Activity (Label = "Login to OpenIZ", NoHistory = true, Theme = "@style/OpenIZ.NoTitle")]			
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

			// Wire up events
			this.m_btnLogin.Click += (o,e)=>
			{
				Intent mainPageIntent = new Intent(ApplicationContext, typeof(HomeActivity));

				this.StartActivity(mainPageIntent);
			};

		}
	}
}

