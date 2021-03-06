﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4016
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FR60.RemoteWorkspace
{


    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://riggvar.net/wcf", ConfigurationName = "RemoteWorkspace.IRemoteWorkspace")]
    public interface IRemoteWorkspace
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://riggvar.net/wcf/IRemoteWorkspace/DBFileExists", ReplyAction = "http://riggvar.net/wcf/IRemoteWorkspace/DBFileExistsResponse")]
        bool DBFileExists(int WorkspaceID, string fn);

        [System.ServiceModel.OperationContractAttribute(Action = "http://riggvar.net/wcf/IRemoteWorkspace/DBDirectoryExists", ReplyAction = "http://riggvar.net/wcf/IRemoteWorkspace/DBDirectoryExistsResponse")]
        bool DBDirectoryExists(int WorkspaceID, string dn);

        [System.ServiceModel.OperationContractAttribute(Action = "http://riggvar.net/wcf/IRemoteWorkspace/DBGetEventNames", ReplyAction = "http://riggvar.net/wcf/IRemoteWorkspace/DBGetEventNamesResponse")]
        string[] DBGetEventNames(int WorkspaceID, string ExtensionFilter);

        [System.ServiceModel.OperationContractAttribute(Action = "http://riggvar.net/wcf/IRemoteWorkspace/DBLoadFromFile", ReplyAction = "http://riggvar.net/wcf/IRemoteWorkspace/DBLoadFromFileResponse")]
        string DBLoadFromFile(int WorkspaceID, string fn);

        [System.ServiceModel.OperationContractAttribute(Action = "http://riggvar.net/wcf/IRemoteWorkspace/DBSaveToFile", ReplyAction = "http://riggvar.net/wcf/IRemoteWorkspace/DBSaveToFileResponse")]
        void DBSaveToFile(int WorkspaceID, string fn, string data);

        [System.ServiceModel.OperationContractAttribute(Action = "http://riggvar.net/wcf/IRemoteWorkspace/DBDeleteFile", ReplyAction = "http://riggvar.net/wcf/IRemoteWorkspace/DBDeleteFileResponse")]
        bool DBDeleteFile(int WorkspaceID, string fn);

        [System.ServiceModel.OperationContractAttribute(Action = "http://riggvar.net/wcf/IRemoteWorkspace/DBDeleteWorkspace", ReplyAction = "http://riggvar.net/wcf/IRemoteWorkspace/DBDeleteWorkspaceResponse")]
        bool DBDeleteWorkspace(int WorkspaceID);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IRemoteWorkspaceChannel : FR60.RemoteWorkspace.IRemoteWorkspace, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class RemoteWorkspaceClient : System.ServiceModel.ClientBase<FR60.RemoteWorkspace.IRemoteWorkspace>, FR60.RemoteWorkspace.IRemoteWorkspace
    {

        public RemoteWorkspaceClient()
        {
        }

        public RemoteWorkspaceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public RemoteWorkspaceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public RemoteWorkspaceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public RemoteWorkspaceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public bool DBFileExists(int WorkspaceID, string fn)
        {
            return base.Channel.DBFileExists(WorkspaceID, fn);
        }

        public bool DBDirectoryExists(int WorkspaceID, string dn)
        {
            return base.Channel.DBDirectoryExists(WorkspaceID, dn);
        }

        public string[] DBGetEventNames(int WorkspaceID, string ExtensionFilter)
        {
            return base.Channel.DBGetEventNames(WorkspaceID, ExtensionFilter);
        }

        public string DBLoadFromFile(int WorkspaceID, string fn)
        {
            return base.Channel.DBLoadFromFile(WorkspaceID, fn);
        }

        public void DBSaveToFile(int WorkspaceID, string fn, string data)
        {
            base.Channel.DBSaveToFile(WorkspaceID, fn, data);
        }

        public bool DBDeleteFile(int WorkspaceID, string fn)
        {
            return base.Channel.DBDeleteFile(WorkspaceID, fn);
        }

        public bool DBDeleteWorkspace(int WorkspaceID)
        {
            return base.Channel.DBDeleteWorkspace(WorkspaceID);
        }
    }
}
