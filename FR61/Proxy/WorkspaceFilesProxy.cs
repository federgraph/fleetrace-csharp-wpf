﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.20506.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.20506.1.
// 
#pragma warning disable 1591

namespace FR60.WorkspaceFiles
{
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "WorkspaceFilesSoap", Namespace = "http://riggvar.net/workspace")]
    public partial class WorkspaceFiles : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback HelloWorldOperationCompleted;

        private System.Threading.SendOrPostCallback DBFileExistsOperationCompleted;

        private System.Threading.SendOrPostCallback DBDirectoryExistsOperationCompleted;

        private System.Threading.SendOrPostCallback DBGetEventNamesOperationCompleted;

        private System.Threading.SendOrPostCallback DBLoadFromFileOperationCompleted;

        private System.Threading.SendOrPostCallback DBSaveToFileOperationCompleted;

        private System.Threading.SendOrPostCallback DBDeleteFileOperationCompleted;

        private System.Threading.SendOrPostCallback DBDeleteWorkspaceOperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

        /// <remarks/>
        public WorkspaceFiles()
        {
            this.Url = System.Configuration.ConfigurationManager.AppSettings["WorkspaceFilesUrl"];
            //this.Url = global::FR60.Properties.Settings.Default.FR60_WorkspaceFiles_WorkspaceFiles;
            if ((this.IsLocalFileSystemWebService(this.Url) == true))
            {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else
            {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true)
                            && (this.useDefaultCredentialsSetExplicitly == false))
                            && (this.IsLocalFileSystemWebService(value) == false)))
                {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        /// <remarks/>
        public event HelloWorldCompletedEventHandler HelloWorldCompleted;

        /// <remarks/>
        public event DBFileExistsCompletedEventHandler DBFileExistsCompleted;

        /// <remarks/>
        public event DBDirectoryExistsCompletedEventHandler DBDirectoryExistsCompleted;

        /// <remarks/>
        public event DBGetEventNamesCompletedEventHandler DBGetEventNamesCompleted;

        /// <remarks/>
        public event DBLoadFromFileCompletedEventHandler DBLoadFromFileCompleted;

        /// <remarks/>
        public event DBSaveToFileCompletedEventHandler DBSaveToFileCompleted;

        /// <remarks/>
        public event DBDeleteFileCompletedEventHandler DBDeleteFileCompleted;

        /// <remarks/>
        public event DBDeleteWorkspaceCompletedEventHandler DBDeleteWorkspaceCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://riggvar.net/workspace/HelloWorld", RequestNamespace = "http://riggvar.net/workspace", ResponseNamespace = "http://riggvar.net/workspace", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string HelloWorld()
        {
            object[] results = this.Invoke("HelloWorld", new object[0]);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void HelloWorldAsync()
        {
            this.HelloWorldAsync(null);
        }

        /// <remarks/>
        public void HelloWorldAsync(object userState)
        {
            if ((this.HelloWorldOperationCompleted == null))
            {
                this.HelloWorldOperationCompleted = new System.Threading.SendOrPostCallback(this.OnHelloWorldOperationCompleted);
            }
            this.InvokeAsync("HelloWorld", new object[0], this.HelloWorldOperationCompleted, userState);
        }

        private void OnHelloWorldOperationCompleted(object arg)
        {
            if ((this.HelloWorldCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.HelloWorldCompleted(this, new HelloWorldCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://riggvar.net/workspace/DBFileExists", RequestNamespace = "http://riggvar.net/workspace", ResponseNamespace = "http://riggvar.net/workspace", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool DBFileExists(int WorkspaceID, string fn)
        {
            object[] results = this.Invoke("DBFileExists", new object[] {
                        WorkspaceID,
                        fn});
            return ((bool)(results[0]));
        }

        /// <remarks/>
        public void DBFileExistsAsync(int WorkspaceID, string fn)
        {
            this.DBFileExistsAsync(WorkspaceID, fn, null);
        }

        /// <remarks/>
        public void DBFileExistsAsync(int WorkspaceID, string fn, object userState)
        {
            if ((this.DBFileExistsOperationCompleted == null))
            {
                this.DBFileExistsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDBFileExistsOperationCompleted);
            }
            this.InvokeAsync("DBFileExists", new object[] {
                        WorkspaceID,
                        fn}, this.DBFileExistsOperationCompleted, userState);
        }

        private void OnDBFileExistsOperationCompleted(object arg)
        {
            if ((this.DBFileExistsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DBFileExistsCompleted(this, new DBFileExistsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://riggvar.net/workspace/DBDirectoryExists", RequestNamespace = "http://riggvar.net/workspace", ResponseNamespace = "http://riggvar.net/workspace", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool DBDirectoryExists(int WorkspaceID, string dn)
        {
            object[] results = this.Invoke("DBDirectoryExists", new object[] {
                        WorkspaceID,
                        dn});
            return ((bool)(results[0]));
        }

        /// <remarks/>
        public void DBDirectoryExistsAsync(int WorkspaceID, string dn)
        {
            this.DBDirectoryExistsAsync(WorkspaceID, dn, null);
        }

        /// <remarks/>
        public void DBDirectoryExistsAsync(int WorkspaceID, string dn, object userState)
        {
            if ((this.DBDirectoryExistsOperationCompleted == null))
            {
                this.DBDirectoryExistsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDBDirectoryExistsOperationCompleted);
            }
            this.InvokeAsync("DBDirectoryExists", new object[] {
                        WorkspaceID,
                        dn}, this.DBDirectoryExistsOperationCompleted, userState);
        }

        private void OnDBDirectoryExistsOperationCompleted(object arg)
        {
            if ((this.DBDirectoryExistsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DBDirectoryExistsCompleted(this, new DBDirectoryExistsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://riggvar.net/workspace/DBGetEventNames", RequestNamespace = "http://riggvar.net/workspace", ResponseNamespace = "http://riggvar.net/workspace", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string DBGetEventNames(int WorkspaceID, string ExtensionFilter)
        {
            object[] results = this.Invoke("DBGetEventNames", new object[] {
                        WorkspaceID,
                        ExtensionFilter});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void DBGetEventNamesAsync(int WorkspaceID, string ExtensionFilter)
        {
            this.DBGetEventNamesAsync(WorkspaceID, ExtensionFilter, null);
        }

        /// <remarks/>
        public void DBGetEventNamesAsync(int WorkspaceID, string ExtensionFilter, object userState)
        {
            if ((this.DBGetEventNamesOperationCompleted == null))
            {
                this.DBGetEventNamesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDBGetEventNamesOperationCompleted);
            }
            this.InvokeAsync("DBGetEventNames", new object[] {
                        WorkspaceID,
                        ExtensionFilter}, this.DBGetEventNamesOperationCompleted, userState);
        }

        private void OnDBGetEventNamesOperationCompleted(object arg)
        {
            if ((this.DBGetEventNamesCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DBGetEventNamesCompleted(this, new DBGetEventNamesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://riggvar.net/workspace/DBLoadFromFile", RequestNamespace = "http://riggvar.net/workspace", ResponseNamespace = "http://riggvar.net/workspace", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string DBLoadFromFile(int WorkspaceID, string fn)
        {
            object[] results = this.Invoke("DBLoadFromFile", new object[] {
                        WorkspaceID,
                        fn});
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void DBLoadFromFileAsync(int WorkspaceID, string fn)
        {
            this.DBLoadFromFileAsync(WorkspaceID, fn, null);
        }

        /// <remarks/>
        public void DBLoadFromFileAsync(int WorkspaceID, string fn, object userState)
        {
            if ((this.DBLoadFromFileOperationCompleted == null))
            {
                this.DBLoadFromFileOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDBLoadFromFileOperationCompleted);
            }
            this.InvokeAsync("DBLoadFromFile", new object[] {
                        WorkspaceID,
                        fn}, this.DBLoadFromFileOperationCompleted, userState);
        }

        private void OnDBLoadFromFileOperationCompleted(object arg)
        {
            if ((this.DBLoadFromFileCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DBLoadFromFileCompleted(this, new DBLoadFromFileCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://riggvar.net/workspace/DBSaveToFile", RequestNamespace = "http://riggvar.net/workspace", ResponseNamespace = "http://riggvar.net/workspace", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void DBSaveToFile(int WorkspaceID, string fn, string data)
        {
            this.Invoke("DBSaveToFile", new object[] {
                        WorkspaceID,
                        fn,
                        data});
        }

        /// <remarks/>
        public void DBSaveToFileAsync(int WorkspaceID, string fn, string data)
        {
            this.DBSaveToFileAsync(WorkspaceID, fn, data, null);
        }

        /// <remarks/>
        public void DBSaveToFileAsync(int WorkspaceID, string fn, string data, object userState)
        {
            if ((this.DBSaveToFileOperationCompleted == null))
            {
                this.DBSaveToFileOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDBSaveToFileOperationCompleted);
            }
            this.InvokeAsync("DBSaveToFile", new object[] {
                        WorkspaceID,
                        fn,
                        data}, this.DBSaveToFileOperationCompleted, userState);
        }

        private void OnDBSaveToFileOperationCompleted(object arg)
        {
            if ((this.DBSaveToFileCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DBSaveToFileCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://riggvar.net/workspace/DBDeleteFile", RequestNamespace = "http://riggvar.net/workspace", ResponseNamespace = "http://riggvar.net/workspace", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool DBDeleteFile(int WorkspaceID, string fn)
        {
            object[] results = this.Invoke("DBDeleteFile", new object[] {
                        WorkspaceID,
                        fn});
            return ((bool)(results[0]));
        }

        /// <remarks/>
        public void DBDeleteFileAsync(int WorkspaceID, string fn)
        {
            this.DBDeleteFileAsync(WorkspaceID, fn, null);
        }

        /// <remarks/>
        public void DBDeleteFileAsync(int WorkspaceID, string fn, object userState)
        {
            if ((this.DBDeleteFileOperationCompleted == null))
            {
                this.DBDeleteFileOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDBDeleteFileOperationCompleted);
            }
            this.InvokeAsync("DBDeleteFile", new object[] {
                        WorkspaceID,
                        fn}, this.DBDeleteFileOperationCompleted, userState);
        }

        private void OnDBDeleteFileOperationCompleted(object arg)
        {
            if ((this.DBDeleteFileCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DBDeleteFileCompleted(this, new DBDeleteFileCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://riggvar.net/workspace/DBDeleteWorkspace", RequestNamespace = "http://riggvar.net/workspace", ResponseNamespace = "http://riggvar.net/workspace", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool DBDeleteWorkspace(int WorkspaceID)
        {
            object[] results = this.Invoke("DBDeleteWorkspace", new object[] {
                        WorkspaceID});
            return ((bool)(results[0]));
        }

        /// <remarks/>
        public void DBDeleteWorkspaceAsync(int WorkspaceID)
        {
            this.DBDeleteWorkspaceAsync(WorkspaceID, null);
        }

        /// <remarks/>
        public void DBDeleteWorkspaceAsync(int WorkspaceID, object userState)
        {
            if ((this.DBDeleteWorkspaceOperationCompleted == null))
            {
                this.DBDeleteWorkspaceOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDBDeleteWorkspaceOperationCompleted);
            }
            this.InvokeAsync("DBDeleteWorkspace", new object[] {
                        WorkspaceID}, this.DBDeleteWorkspaceOperationCompleted, userState);
        }

        private void OnDBDeleteWorkspaceOperationCompleted(object arg)
        {
            if ((this.DBDeleteWorkspaceCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DBDeleteWorkspaceCompleted(this, new DBDeleteWorkspaceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (((url == null)
                        || (url == string.Empty)))
            {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024)
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0)))
            {
                return true;
            }
            return false;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    public delegate void HelloWorldCompletedEventHandler(object sender, HelloWorldCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class HelloWorldCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal HelloWorldCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    public delegate void DBFileExistsCompletedEventHandler(object sender, DBFileExistsCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DBFileExistsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DBFileExistsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public bool Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    public delegate void DBDirectoryExistsCompletedEventHandler(object sender, DBDirectoryExistsCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DBDirectoryExistsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DBDirectoryExistsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public bool Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    public delegate void DBGetEventNamesCompletedEventHandler(object sender, DBGetEventNamesCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DBGetEventNamesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DBGetEventNamesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    public delegate void DBLoadFromFileCompletedEventHandler(object sender, DBLoadFromFileCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DBLoadFromFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DBLoadFromFileCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    public delegate void DBSaveToFileCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    public delegate void DBDeleteFileCompletedEventHandler(object sender, DBDeleteFileCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DBDeleteFileCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DBDeleteFileCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public bool Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    public delegate void DBDeleteWorkspaceCompletedEventHandler(object sender, DBDeleteWorkspaceCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.20506.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DBDeleteWorkspaceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal DBDeleteWorkspaceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public bool Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591