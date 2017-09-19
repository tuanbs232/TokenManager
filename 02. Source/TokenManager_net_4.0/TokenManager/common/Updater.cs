using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using TokenManager.dialog;
using TokenManager.tmsclient;

namespace TokenManager.common
{
    class Updater
    {
        private static readonly log4net.ILog _LOG = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);
        static MainWindow _mainWindow;
        internal static void CheckForUpdate(MainWindow _main)
        {
            _LOG.Info("CheckForUpdate: Check for new software version");
            _mainWindow = _main;
            ThreadStart child = new ThreadStart(HandleCheckUpdate);
            Thread parent = new Thread(child);
            parent.Start();
            LanguageUtil lang = LanguageUtil.GetInstance();
            _main.Update(lang.GetValue(LanguageUtil.Key.UPDATE_CHECKING_NEW_VERSION), CommonMessage.MESSAGE_TYPE_ACTION_WITH_LOADING, null);
        }

        internal static void CheckForUpdateSilent(MainWindow _main)
        {
            _LOG.Info("CheckForUpdate: Startup check for new software version");
            _mainWindow = _main;
            ThreadStart child = new ThreadStart(HandleCheckUpdateSilent);
            Thread parent = new Thread(child);
            parent.Start();
        }


        private static void HandleCheckUpdate()
        {
            int result = TMSClient.CheckForUpdate();
            LanguageUtil lang = LanguageUtil.GetInstance();
            if (TMSClient.SUCCESSFULL == result)
            {
                _LOG.Info("HandleCheckUpdate: Have new software version");
                UpdateMeta data = TMSClient.GetUpdateData();
                _mainWindow.InvokeConfirmDialog(lang.GetValue(LanguageUtil.Key.UPDATE_HAVE_NEW_VERSION).Replace("[VER]", data.version), CommonMessage.UPDATE_DOWNLOAD_UPDATE_PACKAGE);
            }
            else if(TMSClient.NO_NEW_VERSION_FOUND == result)
            {
                _LOG.Info("HandleCheckUpdate: Have NO new software version");
                _mainWindow.InvokeMessageDialog(lang.GetValue(LanguageUtil.Key.UPDATE_NO_NEW_VERSION));
            }
            else
            {
                string error = TMSClient.GetErrorMessage(result);
                _LOG.Info("HandleCheckUpdate: Get an exception=" + error);
                _mainWindow.InvokeErrorDialog(error);
            }
        }

        private static void HandleCheckUpdateSilent()
        {
            int result = TMSClient.CheckForUpdate();
            LanguageUtil lang = LanguageUtil.GetInstance();
            if (TMSClient.SUCCESSFULL == result)
            {
                _LOG.Info("HandleCheckUpdateSilent: Have new software version");
                UpdateMeta data = TMSClient.GetUpdateData();
                _mainWindow.InvokeConfirmDialog(lang.GetValue(LanguageUtil.Key.UPDATE_HAVE_NEW_VERSION).Replace("[VER]", data.version), CommonMessage.UPDATE_DOWNLOAD_UPDATE_PACKAGE);
            }
            else
            {
                _LOG.Info("HandleCheckUpdateSilent: Have NO new software version");
            }
        }

        static UpdateDialog _dialog;
        public static void DownloadUpdatePackage(WebClient client, UpdateDialog dialog)
        {
            _dialog = dialog;
            _LOG.Info("DownloadUpdatePackage: Download update package.");
            UpdateMeta data = TMSClient.GetUpdateData();
            if(data == null || data.linkDownload == null)
            {
                _LOG.Error("DownloadUpdatePackage: No download link in meta data");
                return;
            }
            Uri uri = null;
            try
            {
                uri = new Uri(data.linkDownload);
            }
            catch(Exception ex)
            {
                _LOG.Error("DownloadUpdatePackage: " + ex.Message);
                return;
            }
            _LOG.Info("DownloadUpdatePackage: Update package uri=" + uri);

            try
            {
                client.DownloadProgressChanged += WebClientDownloadProgressChanged;
                client.DownloadFileCompleted += WebClientDownloadCompleted;
                string tmpDir = System.IO.Path.GetTempPath() + "\\";
                client.DownloadFileAsync(uri, tmpDir + TokenManagerConstants.UPDATE_PACKAGE_TMP_PATH);
            }
            catch(Exception ex)
            {
                _LOG.Error("DownloadUpdatePackage: " + ex.Message);
                _dialog.InvokeErrorMessage(ex.Message);
                return;
            }
        }

        private static void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _dialog.InvokeChangeProgressBar(e.ProgressPercentage, e.TotalBytesToReceive, e.BytesReceived);
        }

        private static void WebClientDownloadCompleted(object sender, AsyncCompletedEventArgs args)
        {
            if (args.Cancelled)
            {
                _LOG.Info("Download cancelled");
                return;
            }
            if(args.Error != null)
            {
                _LOG.Error("Download complete with exception: " + args.Error.Message);
                string message = LanguageUtil.GetInstance().GetValue(LanguageUtil.Key.UPDATE_CANNOT_DOWNLOAD_PACKAGE);
                _dialog.InvokeErrorMessage(message);
                return;
            }
            _LOG.Info("Download update package completely");
            _dialog.InvokeDownloadComplete();
        }

    }
}
