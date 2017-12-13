namespace MWindowInterfacesLib.MsgBox
{
    using Enums;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an interface to a message box service that can
    /// display message boxes in a variety of different configurations.
    /// </summary>
    public interface IMessageBoxService
    {
        #region IMsgBoxService methods
        #region Simple Messages
        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// Only the <paramref name="messageBoxText"/> is a required parameter
        /// all others are optional.
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(
              string messageBoxText
            , MsgBoxResult btnDefault = MsgBoxResult.None
            , object helpLink = null
            , string helpLinkTitle = ""
            , string helpLabel = ""
            , Func<object, bool> navigateHelplinkMethod = null
            , bool showCopyMessage = false
            );

        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// Only the <paramref name="messageBoxText"/> is a required parameter
        /// all others are optional.
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        MsgBoxResult Show(
              string messageBoxText
            , MsgBoxResult btnDefault = MsgBoxResult.None
            , object helpLink = null
            , string helpLinkTitle = ""
            , string helpLabel = ""
            , Func<object, bool> navigateHelplinkMethod = null
            , bool showCopyMessage = false
            );

        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// Only the <paramref name="messageBoxText"/> and 
        /// <paramref name="caption"/> are a required parameters
        /// all others are optional.
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(
            string messageBoxText
          , string caption
          , MsgBoxResult btnDefault = MsgBoxResult.None
          , object helpLink = null
          , string helpLinkTitle = ""
          , string helpLabel = ""
          , Func<object, bool> navigateHelplinkMethod = null
          , bool showCopyMessage = false);

        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// Only the <paramref name="messageBoxText"/> and 
        /// <paramref name="caption"/> are a required parameters
        /// all others are optional.
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        MsgBoxResult Show(
            string messageBoxText
          , string caption
          , MsgBoxResult btnDefault = MsgBoxResult.None
          , object helpLink = null
          , string helpLinkTitle = ""
          , string helpLabel = ""
          , Func<object, bool> navigateHelplinkMethod = null
          , bool showCopyMessage = false);

        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// The parameters:
        /// <paramref name="messageBoxText"/> and 
        /// <paramref name="caption"/>
        /// <paramref name="buttonOption"/>
        /// are a required parameters.
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(string messageBoxText, string caption,
          MsgBoxButtons buttonOption,
          MsgBoxResult btnDefault = MsgBoxResult.None,
          object helpLink = null,
          string helpLinkTitle = "",
          string helpLabel = "",
          Func<object, bool> navigateHelplinkMethod = null,
          bool showCopyMessage = false);

        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// The parameters:
        /// <paramref name="messageBoxText"/> and 
        /// <paramref name="caption"/>
        /// <paramref name="buttonOption"/>
        /// are a required parameters.
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        MsgBoxResult Show(string messageBoxText, string caption,
          MsgBoxButtons buttonOption,
          MsgBoxResult btnDefault = MsgBoxResult.None,
          object helpLink = null,
          string helpLinkTitle = "",
          string helpLabel = "",
          Func<object, bool> navigateHelplinkMethod = null,
          bool showCopyMessage = false);

        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// The parameters:
        /// <paramref name="messageBoxText"/> and 
        /// <paramref name="caption"/>
        /// <paramref name="buttonOption"/>
        /// <param name="image"></param>
        /// are a required parameters.
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(string messageBoxText, string caption,
                          MsgBoxButtons buttonOption, MsgBoxImage image,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// The parameters:
        /// <paramref name="messageBoxText"/> and 
        /// <paramref name="caption"/>
        /// <paramref name="buttonOption"/>
        /// <param name="image"></param>
        /// are a required parameters.
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        MsgBoxResult Show(string messageBoxText, string caption,
                          MsgBoxButtons buttonOption, MsgBoxImage image,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// The parameters:
        /// <paramref name="messageBoxText"/> and 
        /// <paramref name="caption"/>
        /// <paramref name="details"/>
        /// <paramref name="buttonOption"/>
        /// <param name="image"></param>
        /// are a required parameters.
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(string messageBoxText, string caption,
                  string details,
                  MsgBoxButtons buttonOption, MsgBoxImage image,
                  MsgBoxResult btnDefault = MsgBoxResult.None,
                  object helpLink = null,
                  string helpLinkTitle = "",
                  string helpLabel = "",
                  Func<object, bool> navigateHelplinkMethod = null,
                  bool showCopyMessage = false);

        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// The parameters:
        /// <paramref name="messageBoxText"/> and 
        /// <paramref name="caption"/>
        /// <paramref name="details"/>
        /// <paramref name="buttonOption"/>
        /// <param name="image"></param>
        /// are a required parameters.
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        MsgBoxResult Show(string messageBoxText, string caption,
                  string details,
                  MsgBoxButtons buttonOption, MsgBoxImage image,
                  MsgBoxResult btnDefault = MsgBoxResult.None,
                  object helpLink = null,
                  string helpLinkTitle = "",
                  string helpLabel = "",
                  Func<object, bool> navigateHelplinkMethod = null,
                  bool showCopyMessage = false);
        #endregion Simple Messages

        #region Messages with display of Exception
        /// <summary>
        /// Show a message box with a standard Exception display to the user.
        /// The parameters:
        /// <paramref name="exp"/>
        /// <paramref name="messageBoxText"/> and 
        /// <paramref name="caption"/>
        /// <paramref name="buttonOption"/>
        /// <param name="image"></param>
        /// are a required parameters.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="caption"></param>
        /// <param name="buttonOption"></param>
        /// <param name="image"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(Exception exp, string caption,
                      MsgBoxButtons buttonOption, MsgBoxImage image,
                      MsgBoxResult btnDefault = MsgBoxResult.None,
                      object helpLink = null,
                      string helpLinkTitle = "",
                      string helpLabel = "",
                      Func<object, bool> navigateHelplinkMethod = null,
                      bool showCopyMessage = false);

        /// <summary>
        /// Show a message box with a standard Exception display to the user.
        /// The parameters:
        /// <paramref name="exp"/>
        /// <paramref name="messageBoxText"/> and 
        /// <paramref name="caption"/>
        /// <paramref name="buttonOption"/>
        /// <param name="image"></param>
        /// are a required parameters.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="caption"></param>
        /// <param name="buttonOption"></param>
        /// <param name="image"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        MsgBoxResult Show(Exception exp, string caption,
                      MsgBoxButtons buttonOption, MsgBoxImage image,
                      MsgBoxResult btnDefault = MsgBoxResult.None,
                      object helpLink = null,
                      string helpLinkTitle = "",
                      string helpLabel = "",
                      Func<object, bool> navigateHelplinkMethod = null,
                      bool showCopyMessage = false);

        /// <summary>
        /// Show a message box with a standard Exception display to the user.
        /// Only the <paramref name="exp"/> parameter is required all others are optional.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="caption"></param>
        /// <param name="buttonOption"></param>
        /// <param name="image"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(Exception exp,
                  string textMessage = "", string caption = "",
                  MsgBoxButtons buttonOption = MsgBoxButtons.OK,
                  MsgBoxImage image = MsgBoxImage.Error,
                  MsgBoxResult btnDefault = MsgBoxResult.None,
                  object helpLink = null,
                  string helpLinkTitle = "",
                  string helpLabel = "",
                  Func<object, bool> navigateHelplinkMethod = null,
                  bool showCopyMessage = false);

        /// <summary>
        /// Show a message box with a standard Exception display to the user.
        /// Only the <paramref name="exp"/> parameter is required all others are optional.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="caption"></param>
        /// <param name="buttonOption"></param>
        /// <param name="image"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        MsgBoxResult Show(Exception exp,
                  string textMessage = "", string caption = "",
                  MsgBoxButtons buttonOption = MsgBoxButtons.OK,
                  MsgBoxImage image = MsgBoxImage.Error,
                  MsgBoxResult btnDefault = MsgBoxResult.None,
                  object helpLink = null,
                  string helpLinkTitle = "",
                  string helpLabel = "",
                  Func<object, bool> navigateHelplinkMethod = null,
                  bool showCopyMessage = false);
        #endregion Messages with display of Exception

        #region Messages with explicit Context or Window Owner Paremeter
        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// Only <param name="ownerContext"/> and <paramref name="messageBoxText"/>
        /// are required parameters
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <param name="ownerContext"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(object ownerContext,
                          string messageBoxText, string caption = "",
                          MsgBoxButtons buttonOption = MsgBoxButtons.OK,
                          MsgBoxImage image = MsgBoxImage.Error,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLinkLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// Only <param name="ownerContext"/> and <paramref name="messageBoxText"/>
        /// are required parameters
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <param name="ownerContext"></param>
        /// <returns></returns>
        MsgBoxResult Show(object ownerContext,
                          string messageBoxText, string caption = "",
                          MsgBoxButtons buttonOption = MsgBoxButtons.OK,
                          MsgBoxImage image = MsgBoxImage.Error,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLinkLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// Only the
        /// <paramref name="ownerContext"/>,
        /// <paramref name="messageBoxText"/>,
        /// <paramref name="caption"/>,
        /// <paramref name="defaultCloseResult"/>,
        /// <paramref name="dialogCanCloseViaChrome"/> are required parameters
        /// XXX
        /// XXX TODO: dialogCanCloseViaChrome and defaultCloseResult are NOT supported yet
        /// XXX
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(object ownerContext,
        string messageBoxText, string caption,
                          MsgBoxResult defaultCloseResult,
                          bool dialogCanCloseViaChrome,
                          MsgBoxButtons buttonOption = MsgBoxButtons.OK,
                          MsgBoxImage image = MsgBoxImage.Error,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLinkLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a simple message (minimal with OK button) to the user.
        /// Only the
        /// <paramref name="ownerContext"/>,
        /// <paramref name="messageBoxText"/>,
        /// <paramref name="caption"/>,
        /// <paramref name="defaultCloseResult"/>,
        /// <paramref name="dialogCanCloseViaChrome"/> are required parameters
        /// XXX
        /// XXX TODO: dialogCanCloseViaChrome and defaultCloseResult are NOT supported yet
        /// XXX
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        MsgBoxResult Show(object ownerContext,
        string messageBoxText, string caption,
                          MsgBoxResult defaultCloseResult,
                          bool dialogCanCloseViaChrome,
                          MsgBoxButtons buttonOption = MsgBoxButtons.OK,
                          MsgBoxImage image = MsgBoxImage.Error,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLinkLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);
        #endregion Messages with explicit Context or Window Owner Paremeter

        #region Explicit defaultCloseResult, dialogCanCloseViaChrome Paremeter (XXX TODO)
        /// <summary>
        /// Show a message dialog to the user.
        /// Only the
        /// <paramref name="messageBoxText"/>,
        /// <paramref name="defaultCloseResult"/>,
        /// <paramref name="dialogCanCloseViaChrome"/> are required parameters
        /// XXX
        /// XXX TODO: dialogCanCloseViaChrome and defaultCloseResult are NOT supported yet
        /// XXX
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="defaultCloseResult"></param>
        /// <param name="dialogCanCloseViaChrome"></param>
        /// 
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLinkLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(string messageBoxText,
                          MsgBoxResult defaultCloseResult,
                          bool dialogCanCloseViaChrome,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a message dialog to the user.
        /// Only the
        /// <paramref name="messageBoxText"/>,
        /// <paramref name="defaultCloseResult"/>,
        /// <paramref name="dialogCanCloseViaChrome"/> are required parameters
        /// XXX
        /// XXX TODO: dialogCanCloseViaChrome and defaultCloseResult are NOT supported yet
        /// XXX
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="defaultCloseResult"></param>
        /// <param name="dialogCanCloseViaChrome"></param>
        /// 
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLinkLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        MsgBoxResult Show(string messageBoxText,
                          MsgBoxResult defaultCloseResult,
                          bool dialogCanCloseViaChrome,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a message dialog to the user.
        /// Only the
        /// <paramref name="messageBoxText"/>,
        /// <paramref name="caption"/>,
        /// <paramref name="defaultCloseResult"/>,
        /// <paramref name="dialogCanCloseViaChrome"/> are required parameters
        /// XXX
        /// XXX TODO: dialogCanCloseViaChrome and defaultCloseResult are NOT supported yet
        /// XXX
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="defaultCloseResult"></param>
        /// <param name="dialogCanCloseViaChrome"></param>
        /// 
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLinkLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        Task<MsgBoxResult> ShowAsync(string messageBoxText, string caption,
                          MsgBoxResult defaultCloseResult,
                          bool dialogCanCloseViaChrome,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a message dialog to the user.
        /// Only the
        /// <paramref name="messageBoxText"/>,
        /// <paramref name="caption"/>,
        /// <paramref name="defaultCloseResult"/>,
        /// <paramref name="dialogCanCloseViaChrome"/> are required parameters
        /// XXX
        /// XXX TODO: dialogCanCloseViaChrome and defaultCloseResult are NOT supported yet
        /// XXX
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="defaultCloseResult"></param>
        /// <param name="dialogCanCloseViaChrome"></param>
        /// 
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLinkLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        MsgBoxResult Show(string messageBoxText, string caption,
                          MsgBoxResult defaultCloseResult,
                          bool dialogCanCloseViaChrome,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a message dialog to the user.
        /// Only the
        /// <paramref name="messageBoxText"/>,
        /// <paramref name="caption"/>,
        /// <paramref name="buttonOption"/>,
        /// <paramref name="defaultCloseResult"/>,
        /// <paramref name="dialogCanCloseViaChrome"/> are required parameters
        /// XXX
        /// XXX TODO: dialogCanCloseViaChrome and defaultCloseResult are NOT supported yet
        /// XXX
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="buttonOption"></param>
        /// <param name="defaultCloseResult"></param>
        /// <param name="dialogCanCloseViaChrome"></param>
        /// 
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLinkLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(string messageBoxText, string caption,
                          MsgBoxButtons buttonOption,
                          MsgBoxResult defaultCloseResult,
                          bool dialogCanCloseViaChrome,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a message dialog to the user.
        /// Only the
        /// <paramref name="messageBoxText"/>,
        /// <paramref name="caption"/>,
        /// <paramref name="buttonOption"/>,
        /// <paramref name="defaultCloseResult"/>,
        /// <paramref name="dialogCanCloseViaChrome"/> are required parameters
        /// XXX
        /// XXX TODO: dialogCanCloseViaChrome and defaultCloseResult are NOT supported yet
        /// XXX
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="buttonOption"></param>
        /// <param name="defaultCloseResult"></param>
        /// <param name="dialogCanCloseViaChrome"></param>
        /// 
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLinkLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        MsgBoxResult Show(string messageBoxText, string caption,
                          MsgBoxButtons buttonOption,
                          MsgBoxResult defaultCloseResult,
                          bool dialogCanCloseViaChrome,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a message dialog to the user.
        /// Only the
        /// <paramref name="messageBoxText"/>,
        /// <paramref name="caption"/>,
        /// <paramref name="buttonOption"/>,
        /// <paramref name="image"/>,
        /// <paramref name="defaultCloseResult"/>,
        /// <paramref name="dialogCanCloseViaChrome"/> are required parameters
        /// XXX
        /// XXX TODO: dialogCanCloseViaChrome and defaultCloseResult are NOT supported yet
        /// XXX
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="buttonOption"></param>
        /// <param name="image"></param>
        /// <param name="defaultCloseResult"></param>
        /// <param name="dialogCanCloseViaChrome"></param>
        /// 
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLinkLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(string messageBoxText, string caption,
                          MsgBoxButtons buttonOption, MsgBoxImage image,
                          MsgBoxResult defaultCloseResult,
                          bool dialogCanCloseViaChrome,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a message dialog to the user.
        /// Only the
        /// <paramref name="messageBoxText"/>,
        /// <paramref name="caption"/>,
        /// <paramref name="buttonOption"/>,
        /// <paramref name="image"/>,
        /// <paramref name="defaultCloseResult"/>,
        /// <paramref name="dialogCanCloseViaChrome"/> are required parameters
        /// XXX
        /// XXX TODO: dialogCanCloseViaChrome and defaultCloseResult are NOT supported yet
        /// XXX
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="buttonOption"></param>
        /// <param name="image"></param>
        /// <param name="defaultCloseResult"></param>
        /// <param name="dialogCanCloseViaChrome"></param>
        /// 
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLinkLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        MsgBoxResult Show(string messageBoxText, string caption,
                          MsgBoxButtons buttonOption, MsgBoxImage image,
                          MsgBoxResult defaultCloseResult,
                          bool dialogCanCloseViaChrome,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);

        /// <summary>
        /// Show a message dialog to the user.
        /// Only the
        /// <paramref name="messageBoxText"/>,
        /// <paramref name="caption"/>,
        /// <paramref name="details"/>,
        /// <paramref name="buttonOption"/>,
        /// <paramref name="image"/>,
        /// <paramref name="defaultCloseResult"/>,
        /// <paramref name="dialogCanCloseViaChrome"/> are required parameters
        /// XXX
        /// XXX TODO: dialogCanCloseViaChrome and defaultCloseResult are NOT supported yet
        /// XXX
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="details"></param>
        /// <param name="buttonOption"></param>
        /// <param name="image"></param>
        /// <param name="defaultCloseResult"></param>
        /// <param name="dialogCanCloseViaChrome"></param>
        /// 
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLinkLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        Task<MsgBoxResult> ShowAsync(string messageBoxText, string caption,
                                     string details,
                                      MsgBoxButtons buttonOption, MsgBoxImage image,
                                      MsgBoxResult defaultCloseResult,
                                      bool dialogCanCloseViaChrome,
                                      MsgBoxResult btnDefault = MsgBoxResult.None,
                                      object helpLink = null,
                                      string helpLinkTitle = "",
                                      string helpLabel = "",
                                      Func<object, bool> navigateHelplinkMethod = null,
                                      bool showCopyMessage = false);

        /// <summary>
        /// Show a message dialog to the user.
        /// Only the
        /// <paramref name="messageBoxText"/>,
        /// <paramref name="caption"/>,
        /// <paramref name="details"/>,
        /// <paramref name="buttonOption"/>,
        /// <paramref name="image"/>,
        /// <paramref name="defaultCloseResult"/>,
        /// <paramref name="dialogCanCloseViaChrome"/> are required parameters
        /// XXX
        /// XXX TODO: dialogCanCloseViaChrome and defaultCloseResult are NOT supported yet
        /// XXX
        /// </summary>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="details"></param>
        /// <param name="buttonOption"></param>
        /// <param name="image"></param>
        /// <param name="defaultCloseResult"></param>
        /// <param name="dialogCanCloseViaChrome"></param>
        /// 
        /// <param name="btnDefault"></param>
        /// <param name="helpLink"></param>
        /// <param name="helpLinkTitle"></param>
        /// <param name="helpLinkLabel"></param>
        /// <param name="navigateHelplinkMethod"></param>
        /// <param name="showCopyMessage"></param>
        /// <returns></returns>
        MsgBoxResult Show(string messageBoxText, string caption,
                          string details,
                          MsgBoxButtons buttonOption, MsgBoxImage image,
                          MsgBoxResult defaultCloseResult,
                          bool dialogCanCloseViaChrome,
                          MsgBoxResult btnDefault = MsgBoxResult.None,
                          object helpLink = null,
                          string helpLinkTitle = "",
                          string helpLabel = "",
                          Func<object, bool> navigateHelplinkMethod = null,
                          bool showCopyMessage = false);
        #endregion Explicit defaultCloseResult, dialogCanCloseViaChrome Paremeter (XXX TODO)
        #endregion IMsgBoxService methods
    }
}
