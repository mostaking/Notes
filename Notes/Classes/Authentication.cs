﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;

namespace Notes.Classes
{
    public class Authentication
    {
        const string appClientId = "1pe4jp1nmsqb19ta0sf4sgrjt5";
        const string poolId = "us-east-1_DNd1jIHmH";
        static RegionEndpoint region = RegionEndpoint.USEast1;
        static bool isSignedIn = false;
        public static string Username { get; set; }

        public static async Task SignUp(string email, string username, string password)
        {  
            AmazonCognitoIdentityProviderClient providerClient = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials()
                , region);

            //Signing Up..
            SignUpRequest signUpRequest = new SignUpRequest()
            {
                ClientId = appClientId,
                Username = username,
                Password = password
            };

            List<AttributeType> attributes = new List<AttributeType>()
            {
                new AttributeType(){Name="email", Value = email}
            };

            signUpRequest.UserAttributes = attributes;

            try
            {
                SignUpResponse response = await providerClient.SignUpAsync(signUpRequest).ConfigureAwait(false);
                Debug.WriteLine("SUCC");
                Username = username;

                MessageBox.Show("An Email Sent With Verification URL");
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }

        public static async Task SignIn(string username, string password, bool isShowError)
        {
            AmazonCognitoIdentityProviderClient providerClient = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials()
                , region);

            CognitoUserPool userPool = new CognitoUserPool(poolId, appClientId, providerClient);
            CognitoUser cognitoUser = new CognitoUser(username, appClientId, userPool, providerClient);

            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = password
            };

            try
            {
                AuthFlowResponse authFlow = await cognitoUser.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
                isSignedIn = true;
                Username = cognitoUser.Username.ToString();
            }
            catch(Exception e)
            {
                if (isShowError)
                    MessageBox.Show(e.Message);
                return;
            }
        }

        public static string GetCurrentUsername()
        {
            AmazonCognitoIdentityProviderClient providerClient = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials()
               , region);

            CognitoUserPool userPool = new CognitoUserPool(poolId, appClientId, providerClient);
           
            Debug.WriteLine(userPool.GetUser().Username);

            return null;
        }

        public static bool GetLoginStatus()
        { 
            return isSignedIn;
        }
    }
}
