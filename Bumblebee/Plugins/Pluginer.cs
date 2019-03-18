﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using BeetleX.FastHttpApi;
using Bumblebee.Plugins;
using Bumblebee.Events;

namespace Bumblebee.Plugins
{
    public class Pluginer
    {
        public Pluginer(Gateway gateway, Routes.UrlRoute urlRoute)
        {
            Gateway = gateway;
            UrlRoute = urlRoute;
        }

        #region requesting

        private ConcurrentDictionary<string, IRequestingHandler> mRequestingHandlerMap = new ConcurrentDictionary<string, IRequestingHandler>();

        private IRequestingHandler[] mRequestingHandlers = new IRequestingHandler[0];

        public PluginInfo[] RequestingInfos => (from a in mRequestingHandlerMap.Values select new PluginInfo(a)).ToArray();

        public void SetRequesting(string name)
        {
            var item = Gateway.PluginCenter.RequestingHandlers.Get(name);
            if (item == null)
            {
                Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Warring, $"gateway {name} requesting handler not found");
            }
            else
            {
                mRequestingHandlerMap[name] = item;
                mRequestingHandlers = mRequestingHandlerMap.Values.ToArray();
                Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Info, $"gateway {UrlRoute?.Url} load {name} requesting handler ");
            }
        }

        public void RemoveRequesting(string name)
        {
            mRequestingHandlerMap.TryRemove(name, out IRequestingHandler item);
            mRequestingHandlers = mRequestingHandlerMap.Values.ToArray();
            Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Info, $"gateway {UrlRoute?.Url} remove {name} requesting handler ");
        }


        public bool Requesting(HttpRequest request, HttpResponse response)
        {
            var items = mRequestingHandlers;
            if (items.Length > 0)
            {
                Events.EventRequestingArgs e = new Events.EventRequestingArgs(request, response, Gateway);
                for (int i = 0; i < items.Length; i++)
                {
                    if (!e.Cancel)
                        items[i].Execute(e);
                }
                return !e.Cancel;
            }
            return true;
        }


        private void ReloadRequesting()
        {
            foreach (var item in mRequestingHandlerMap.Keys)
            {
                SetRequesting(item);
            }
        }


        #endregion


        #region agent requesting
        private ConcurrentDictionary<string, IAgentRequestingHandler> mAgentRequestingHandlerMap = new ConcurrentDictionary<string, IAgentRequestingHandler>();

        private IAgentRequestingHandler[] mAgentRequestingHandlers = new IAgentRequestingHandler[0];

        public PluginInfo[] AgentRequestingInfos => (from a in mAgentRequestingHandlerMap.Values select new PluginInfo(a)).ToArray();

        public void SetAgentRequesting(string name)
        {
            var item = Gateway.PluginCenter.AgentRequestingHandler.Get(name);
            if (item == null)
            {
                Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Warring, $"gateway {name} agent requesting handler not found");
            }
            else
            {
                mAgentRequestingHandlerMap[name] = item;
                mAgentRequestingHandlers = mAgentRequestingHandlerMap.Values.ToArray();
                Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Info, $"gateway {UrlRoute?.Url} load {name} agent requesting handler ");
            }
        }

        public void RemoveAgentRequesting(string name)
        {
            mAgentRequestingHandlerMap.TryRemove(name, out IAgentRequestingHandler item);
            mAgentRequestingHandlers = mAgentRequestingHandlerMap.Values.ToArray();
            Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Info, $"gateway {UrlRoute?.Url} remove {name} agent requesting handler ");
        }

        public bool AgentRequesting(HttpRequest request, HttpResponse response, Servers.ServerAgent server, Routes.UrlRoute urlRoute)
        {
            var items = mAgentRequestingHandlers;
            if (items.Length > 0)
            {
                Events.EventAgentRequestingArgs e = new Events.EventAgentRequestingArgs(request, response, Gateway, server, urlRoute);
                for (int i = 0; i < items.Length; i++)
                {
                    if (!e.Cancel)
                        items[i].Execute(e);
                }
                return !e.Cancel;
            }
            return true;
        }

        private void ReloadAgentRequesting()
        {
            foreach (var item in mAgentRequestingHandlerMap.Keys)
            {
                SetAgentRequesting(item);
            }
        }

        #endregion



        #region header writing

        private ConcurrentDictionary<string, IHeaderWritingHandler> mHeaderWritingHandlerMap = new ConcurrentDictionary<string, IHeaderWritingHandler>();

        private IHeaderWritingHandler[] mHeaderWritingHandlers = new IHeaderWritingHandler[0];

        public PluginInfo[] HeaderWritingInfos => (from a in mHeaderWritingHandlerMap.Values select new PluginInfo(a)).ToArray();

        public void SetHeaderWriting(string name)
        {
            var item = Gateway.PluginCenter.HeaderWritingHandlers.Get(name);
            if (item == null)
            {
                Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Warring, $"gateway {name} header writing handler not found");
            }
            else
            {
                mHeaderWritingHandlerMap[name] = item;
                mHeaderWritingHandlers = mHeaderWritingHandlerMap.Values.ToArray();
                Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Info, $"gateway {UrlRoute?.Url} load {name} header writing handler ");
            }
        }

        public void RemoveHeaderWriting(string name)
        {
            mHeaderWritingHandlerMap.TryRemove(name, out IHeaderWritingHandler item);
            mHeaderWritingHandlers = mHeaderWritingHandlerMap.Values.ToArray();
            Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Info, $"gateway {UrlRoute?.Url} remove {name} header writing handler ");
        }

        public void HeaderWriting(HttpRequest request, HttpResponse response, Header header)
        {
            var items = mHeaderWritingHandlers;
            if (items.Length > 0)
            {
                Events.EventHeaderWritingArgs e = new Events.EventHeaderWritingArgs(request, response, Gateway, header);
                for (int i = 0; i < items.Length; i++)
                {
                    items[i].Execute(e);
                }
            }
        }

        private void ReloadHeaderWriting()
        {
            foreach (var item in mHeaderWritingHandlerMap.Keys)
            {
                SetHeaderWriting(item);
            }
        }

        #endregion


        #region requested

        private ConcurrentDictionary<string, IRequestedHandler> mRequestedHandlerMap = new ConcurrentDictionary<string, IRequestedHandler>();

        private IRequestedHandler[] mRequestedHandlers = new IRequestedHandler[0];

        public PluginInfo[] RequestedInfos => (from a in mRequestedHandlerMap.Values select new PluginInfo(a)).ToArray();

        public void SetRequested(string name)
        {
            var item = Gateway.PluginCenter.RequestedHandlers.Get(name);
            if (item == null)
            {
                Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Warring, $"gateway {name} requested handler not found");
            }
            else
            {
                mRequestedHandlerMap[name] = item;
                mRequestedHandlers = mRequestedHandlerMap.Values.ToArray();
                Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Info, $"gateway {UrlRoute?.Url} load {name} requested handler ");
            }
        }

        public void RemoveRequested(string name)
        {
            mRequestedHandlerMap.TryRemove(name, out IRequestedHandler item);
            mRequestedHandlers = mRequestedHandlerMap.Values.ToArray();
            Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Info, $"gateway {UrlRoute?.Url} remove {name} requested handler ");
        }

        public void Requested(Servers.RequestAgent requestAgent)
        {
            try
            {
                var items = mRequestedHandlers;
                if (items.Length > 0)
                {
                    Events.EventRequestCompletedArgs e = new Events.EventRequestCompletedArgs(requestAgent.UrlRoute,
                        requestAgent.Request, requestAgent.Response, Gateway, requestAgent.Code, requestAgent.Server, requestAgent.Time);
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i].Execute(e);
                    }
                }
            }
            catch (Exception e_)
            {
                if (Gateway.HttpServer.EnableLog(BeetleX.EventArgs.LogType.Error))
                {
                    Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Error, $"gateway {UrlRoute?.Url} process requeted error {e_.Message}{e_.StackTrace}");
                }
            }

        }

        private void ReloadRequested()
        {
            foreach (var item in mRequestedHandlerMap.Keys)
            {
                SetRequested(item);
            }
        }

        #endregion


        #region response error

        private ConcurrentDictionary<string, IResponseErrorHandler> mResponseErrorHandlerMap = new ConcurrentDictionary<string, IResponseErrorHandler>();

        private IResponseErrorHandler[] responseErrorHandlers = new IResponseErrorHandler[0];

        public PluginInfo[] ResponseErrorInfos => (from a in mResponseErrorHandlerMap.Values select new PluginInfo(a)).ToArray();

        public void SetResponseError(string name)
        {
            var item = Gateway.PluginCenter.ResponseErrorHandlers.Get(name);
            if (item == null)
            {
                Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Warring, $"gateway {name} response handler not found");
            }
            else
            {
                mResponseErrorHandlerMap[name] = item;
                responseErrorHandlers = mResponseErrorHandlerMap.Values.ToArray();
                Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Info, $"gateway {UrlRoute?.Url} load {name} response error handler ");
            }

        }
        public void RemoveResponseError(string name)
        {
            mResponseErrorHandlerMap.TryRemove(name, out IResponseErrorHandler value);
            responseErrorHandlers = mResponseErrorHandlerMap.Values.ToArray();
            Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Info, $"gateway {UrlRoute?.Url} remove {name} response error handler ");
        }

        public void ResponseError(EventResponseErrorArgs e)
        {
            var items = responseErrorHandlers;
            if (items.Length > 0)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    items[i].Exeucte(e);
                }
            }
        }

        private void ReloadResponseError()
        {
            foreach (var item in mResponseErrorHandlerMap.Keys)
            {
                SetResponseError(item);
            }
        }

        #endregion

        public Routes.UrlRoute UrlRoute { get; private set; }

        public Gateway Gateway { get; private set; }


        public void Reload()
        {
            this.ReloadResponseError();
            this.ReloadAgentRequesting();
            this.ReloadHeaderWriting();
            this.ReloadRequested();
            this.ReloadRequesting();
        }
    }
}
