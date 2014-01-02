This is a more complete sample application for the RM Unify SDK for .NET.

The basic application is a very simple school blogging platform.  Multiple schools can sign up, and each can use individual authentication or RM Unify.  Admins and staff members can create new blog posts.

You can find the code that has been modified to support RM Unify by searching for the string "RMUNIFY" (case sensitive).

The majority of the code is in the "Helpers" folder.  There are three different integration implementations in this folder, each of which extends the previous one:

1. RmUnify.cs implements basic RM Unify sign on and sign off.

2. RmUnifyWithLoginNames.cs extends RmUnify.cs to allow meaningful usernames, rather than random strings, while avoiding collisions between your app usernames and RM Unify usernames.  This is useful in the case where your application shows usernames in the UI.

3. RmUnifyWithAccountLinking.cs further externds RmUnifyWithLoginNames.cs to support RM Unify SSO connector licensing (http://dev.rmunify.com/reference/supporting-sso-connector-licensing.aspx) and the RM Unify user account matching process (http://dev.rmunify.com/reference/supporting-user-account-matching.aspx).

The RM Unify SDK is automatically downloaded from nuget.org (as are other packages).  To successfully compile this app, you will need to enable package restore in Visual Studio (see "Enabling Package Restore During Build" in http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages).

Steps to run this sample app:

1. Request an RM Unify development establishment from RM.
2. In your development environment, install the RM Unify Dashboard tile (from the App Library) and click on it in the Launch Pad.
3. Add an app with App Launch URL "http://localhost:64026/", SSO Technology set to "WS-Federation", Realm as any URL you own and Reply To URL as "http://localhost:64026/RmUnify/Index".
4. Go back into the App Library and install your new app for all roles in your school.
5. In Helpers\RmUnify.cs, change the value of Realm to be whatever you chose above.
6. Run the app and click on sign in.