﻿using Mod.ModHelper;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Mod
{
    /// <summary>
    /// Định nghĩa các sự kiện của game.
    /// </summary>
    /// <remarks>
    /// - Các hàm bool trả về true thì sự kiện game sẽ không được thực hiện, 
    /// trả về false thì sự kiện sẽ được kích hoạt như bình thường.<br/>
    /// - Các hàm void hỗ trợ thực hiện các lệnh cùng với sự kiện.
    /// </remarks>
    public static class GameEvents
    {
        /// <summary>
        /// Kích hoạt khi người chơi chat.
        /// </summary>
        /// <param name="text">Nội dung chat.</param>
        /// <returns></returns>
        public static bool onSendChat(string text)
        {
            HistoryChat.gI.append(text);
            bool result = ChatCommandHandler.handleChatText(text);

            return result;
        }

        /// <summary>
        /// Kích hoạt sau khi game khởi động.
        /// </summary>
        public static void onGameStarted()
        {
            ChatCommandHandler.loadDefalut();
            HotkeyCommandHandler.loadDefalut();
            SocketClient.gI.initSender();
        }

        /// <summary>
        /// Kích hoạt khi game đóng
        /// </summary>
        /// <returns></returns>
        public static bool onGameClosing()
        {
            SocketClient.gI.close();
            return false;
        }

        public static void onSaveRMSString(ref string filename, ref string data)
        {
            if (filename is "acc" or "pass")
                data = "pk9r327";
        }

        /// <summary>
        /// Kích hoạt sau khi load KeyMap.
        /// </summary>
        /// <param name="h"></param>
        public static void onKeyMapLoaded(Hashtable h)
        {
        }

        /// <summary>
        /// Kích hoạt khi cài đăt kích thước màn hình.
        /// </summary>
        /// <returns></returns>
        public static bool onSetResolution()
        {
            if (Utilities.sizeData != null)
            {
                int width = (int)Utilities.sizeData["width"];
                int height = (int)Utilities.sizeData["height"];
                Screen.SetResolution(width, height, fullscreen: false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kích hoạt khi nhấn phím tắt (GameScr) chưa được xử lý.
        /// </summary>
        public static void onGameScrPressHotkeysUnassigned()
        {
            HotkeyCommandHandler.handleHotkey(GameCanvas.keyAsciiPress);
        }

        /// <summary>
        /// Kích hoạt sau khi vẽ khung chat.
        /// </summary>
        /// <param name="g"></param>
        public static void onPaintChatTextField(mGraphics g)
        {
            HistoryChat.gI.paint(g);
        }

        /// <summary>
        /// Kích hoạt khi mở khung chat.
        /// </summary>
        public static bool onStartChatTextField(ChatTextField sender)
        {
            if (sender == ChatTextField.gI())
            {
                HistoryChat.gI.show();
            }

            return false;
        }

        public static bool onLoadRMSInt(string file, out int result)
        {
            if (file == "lowGraphic" && Utilities.sizeData != null)
            {
                result = (int)Utilities.sizeData["lowGraphic"];
                return true;
            }

            result = -1;
            return false;
        }

        internal static bool onGetRMSPath(out string result)
        {
            if (Utilities.server != null)
            {
                GameMidlet.IP = (string)Utilities.server["ip"];
                GameMidlet.PORT = (int)Utilities.server["port"];
            }
            result = $"asset\\{GameMidlet.IP}_{GameMidlet.PORT}_x{mGraphics.zoomLevel}\\";
            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }
            return true;
        }

        /// <summary>
        /// Kích hoạt khi có ChatTextField update.
        /// </summary>
        public static void onUpdateChatTextField(ChatTextField sender)
        {
        }

        public static bool onClearAllRMS()
        {
            FileInfo[] files = new DirectoryInfo(Rms.GetiPhoneDocumentsPath() + "/").GetFiles();
            foreach (FileInfo fileInfo in files) 
                if (fileInfo.Name != "isPlaySound")
                    fileInfo.Delete();

            return true;
        }

        /// <summary>
        /// Kích hoạt khi GameScr.gI() update.
        /// </summary>
        public static void onUpdateGameScr()
        {
            Char.myCharz().cspeed = Utilities.speedRun;

            //NOTE onUpdateChatTextField không thể bấm tab.
            HistoryChat.gI.update(); 
        }

        /// <summary>
        /// Kích hoạt khi gửi yêu cầu đăng nhập.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pass"></param>
        public static void onLogin(ref string username, ref string pass)
        {
            username = Utilities.username == "" ? username : Utilities.username;
            pass = Utilities.password == "" ? pass : Utilities.password;
        }

        /// <summary>
        /// Kích hoạt sau khi màn hình chọn server được load.
        /// </summary>
        public static void onServerListScreenLoaded()
        {
            if (GameCanvas.loginScr == null)
            {
                GameCanvas.loginScr = new LoginScr();
            }

            GameCanvas.loginScr.switchToMe();
            Service.gI().login("", "", GameMidlet.VERSION, 0);
            GameCanvas.startWaitDlg();
        }

        /// <summary>
        /// Kích hoạt khi Session kết nối đến server.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public static void onSessionConnecting(ref string host, ref int port)
        {
            if (Utilities.server != null)
            {
                host = (string)Utilities.server["ip"];
                port = (int)Utilities.server["port"];
            }
        }

        public static void onSceenDownloadDataShow()
        {
            GameCanvas.serverScreen.perform(2, null);
        }

        public static bool onCheckZoomLevel()
        {
            if (Utilities.sizeData != null)
            {
                mGraphics.zoomLevel = (int)Utilities.sizeData["typeSize"];
                return true;
            }
            return false;
        }

        public static bool onKeyPressedz(int keyCode, bool isFromSync)
        {
            if (Utilities.channelSyncKey != -1 && !isFromSync)
            {
                SocketClient.gI.sendMessage(new 
                {
                    action = "syncKeyPressed",
                    keyCode,
                    Utilities.channelSyncKey
                });
            }
            return false;
        }

        public static bool onKeyReleasedz(int keyCode, bool isFromAsync)
        {
            if (Utilities.channelSyncKey != -1 && !isFromAsync)
            {
                SocketClient.gI.sendMessage(new
                {
                    action = "syncKeyReleased",
                    keyCode,
                    Utilities.channelSyncKey
                });
            }
            return false;
        }

        public static bool onChatPopupMultiLine(string chat)
        {
            if (chat.ToLower().Contains("chưa thể chuyển khu"))
            {
                GameScr.info1.addInfo(chat, 0);
                return true;
            }

            return false;
        }

        public static void onInfoMapLoaded()
        {
            Utilities.updateWaypointChangeMap();
        }
    }
}