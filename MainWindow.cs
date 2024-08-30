//using System;
using System.Diagnostics;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using System.Xml.Linq;
namespace oobe
{
    public class MainWindow : Window
    {
        [UI] private Label _label1 = null;
        [UI] private Label _label2 = null;
        [UI] private Label _liveinfolabel = null;
        [UI] private Button _button1 = null;
        [UI] private Button _button2 = null;
        [UI] private Image _thum = null;
        [UI] private Image _live_icon = null;
        [UI] private Image texteditoricon = null;
        [UI] private ProgressBar _progress = null;
        [UI] private Box packagelistbox = null;
        [UI] private ComboBoxText texteditorcombo = null;
        [UI] private TextView eulaview = null;
        [UI] private ScrolledWindow windoweulaview = null;
        [UI] private CheckButton toggleswitch = null;
        [UI] private ButtonBox _navigationbar = null;
        [UI] private Box _navi_install = null;
        [UI] private Button _calamares = null;
        [UI] private Label _pionever = null;
        [UI] private Button _liveenv = null;
        [UI] private Button _window_info = null;
        [UI] private Box _infomationbar = null;
        [UI] private Spinner _progresscircle = null;
        private int _state = 0;
        private bool loginview = false;
        private bool windowstimesync = false;
        private bool packagelisted = false;
        private int _bypass_network = 0;
        private int eulaviewed = 0;
        // インストールするパッケージの指定用変数
        private XElement texteditorlistroot = null!;
        //
        public MainWindow() : this(new Builder("MainWindow.glade")) { }
        
        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {

            builder.Autoconnect(this);
            DeleteEvent += Window_DeleteEvent;
                // OSバージョンの確認
                string[] lines = File.ReadAllLines("/etc/os-release");
                string prettyNameLine = lines.FirstOrDefault(line => line.StartsWith("VERSION="));
                string prettyName = prettyNameLine.Split('=')[1].Trim('"');
                _pionever.Text = prettyName;
            // CSS Apply
                var provider = new CssProvider();
                var cssdata = "._navi_install {    background-color: #E0E0E0;       padding: 10px 10px 10px 10px;}._navigationbar{    background-color: #E0E0E0;    padding: 5px 5px 5px 5px} .border{    padding: 1px 0px 1px 0px} ";
                provider.LoadFromData(cssdata);
                StyleContext.AddProviderForScreen(Gdk.Screen.Default, provider, 800); 
            // Live 環境かどうかのチェック
            if (Directory.Exists("/live"))
            {
                _label2.Text = "PioneOS ライブメディアへようこそ";
                _thum.Visible = true;
                _navigationbar.Visible = false;
                _label1.Visible = false;
                _progress.Visible = false;
                windoweulaview.Visible = false;
                toggleswitch.Visible = false;
                _button2.Visible = false;
                _progresscircle.Visible = false;
                _navi_install.Visible = true;
                _liveinfolabel.Visible = true;
                packagelistbox.Visible = false;
                _liveinfolabel.Text = "こんにちは、何をしたいですか？";
                var image = new Gdk.Pixbuf("/usr/share/pioneos/oobe/Assets/pioneos.png",48,48);
                _live_icon.Pixbuf = image;
                _calamares.Clicked += _calamares_Clicked;
                _liveenv.Clicked += _liveenv_Clicked;
                _window_info.Clicked += _window_info_Clicked; 
                Console.WriteLine("Live 環境です");
                _thum.Visible = false;
                //_thum.File = "/usr/share/pioneos/oobe/Assets/pioneos.png";
            }
            else{
                Console.WriteLine("非 Live 環境です");
                _button1.Clicked += Button1_Clicked;
                _button2.Clicked += Button2_Clicked;
                _progress.Visible = false;
                windoweulaview.Visible = false;
                toggleswitch.Visible = false;
                _button2.Visible = false;
                _progresscircle.Visible = false;
                _navi_install.Visible = false;
                _liveinfolabel.Visible = false;
                _live_icon.Visible = true;
                _window_info.Visible = false;
                packagelistbox.Visible = false;
                _thum.File = "/usr/share/pioneos/oobe/Assets/pioneos.png";
            }
            this.Decorated = false;
            this.DeleteEvent += (o, args) => args.RetVal = true;

        }

        private void Button2_Clicked(object sender, EventArgs e)
        {
            if (_bypass_network == 7 && _state == 0){
                _state = 4;
                _button2.Opacity = 1.0;
                _button2.Label = "＜戻る";
                setAsync(sender, e);
            }
            else if (_state == 3){
                var dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok, "エンドユーザーライセンスに同意しない場合は、PioneOS をご利用できません。\n初期ページに戻ります");
                dialog.Run();
                dialog.Destroy();
                windoweulaview.Visible = false;
                _thum.Visible = true;
                _state = 0;
                setAsync(sender, e);
            }
            else {
                _state--;
                setAsync(sender, e);
            }
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            if (_state <= 4)
            {
                Dialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "このウィンドウは閉じれません。\nウィンドウを切り替えるには Alt + Tab を押します。");
                dialog.Run();
                dialog.Destroy();
                a.RetVal = true;
            }

        }
        private async void Button1_Clicked(object sender, EventArgs a)
        {
            _button2.Label = "＜戻る";
            _button2.Opacity = 1.0;
            _button1.Label = "次へ＞";

            _state++;
            setAsync(sender, a);

        }
        private void logintoggled(object sender,EventArgs a)
        {
            //if (_state == 4){
            //    if (toggleswitch.Active == true){
            //        _thum.File = "/usr/share/pioneos/oobe/Assets/usernameoff.png";
            //        loginview = false;
            //    }
            //    else{
            //        _thum.File = "/usr/share/pioneos/oobe/Assets/usernameon.png";
            //        loginview = true;
            //    }}
            if (_state == 4){
                if (toggleswitch.Active == true){
                    windowstimesync = true;
                }
                else{
                    windowstimesync = false;
                }
            }
        }
        //Calamares を起動する
        private async void _calamares_Clicked(object sender,EventArgs a)
        {
            _progresscircle.Visible = true;
            _navi_install.Visible = false;
            _window_info.Visible = false;
            await Task.Delay(2);
            var processInfo_calamares = new ProcessStartInfo
            {
                FileName = "sudo",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = "calamares"
            };
            _progresscircle.Visible = true;
            _navi_install.Visible = false;
            _window_info.Visible = false;
            await Task.Delay(2);
            using (Process pwatch = Process.Start(processInfo_calamares))
            {
                if (pwatch != null)
                {
                    await pwatch.WaitForExitAsync();
                }
            }
            _progresscircle.Visible = false;
            _navi_install.Visible = true;
            _window_info.Visible = true;
            Console.WriteLine("end"); 
            Application.Invoke(delegate {Deiconify();});
        } 
        private async void _liveenv_Clicked(object sender,EventArgs a)
        {
            _progresscircle.Visible = true;
            _navi_install.Visible = false;
            _window_info.Visible = false;
            await Task.Delay(2);
            var processInfo_calamares = new ProcessStartInfo
            {
                FileName = "sudo",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = "/usr/share/pioneos/oobe/livetodesktop.sh"
            };
            _progresscircle.Visible = true;
            _navi_install.Visible = false;
            _window_info.Visible = false;
            await Task.Delay(2);
            Process.Start(processInfo_calamares);
        } 
        private async void _window_info_Clicked(object sender, EventArgs a)
        {
            AboutDialog aboutdialog = new AboutDialog();
            aboutdialog.ProgramName = "PioneOS Setup";
            byte[] epreview = File.ReadAllBytes("/usr/share/pioneos/oobe/Assets/pioneos.png");
            var image = new Gdk.Pixbuf(epreview,128,128);
            aboutdialog.Logo = image;
            aboutdialog.Title = "PioneOS Setup";
            aboutdialog.Comments = "PioneOS をセットアップするソフトウェアです。";
            aboutdialog.Copyright = "(C)2024 PioneOS Group";
            aboutdialog.Authors = new string[] { "Budobudou" };
//            aboutdialog.License = "this software uses gtksharp";
            aboutdialog.Run();
            aboutdialog.ShowAll();
            aboutdialog.Destroy();
        }
        private async Task setAsync(object sender, EventArgs a)
        {
            Console.WriteLine(_state);
            if (_state == 0){
                _label2.Text = "Welcome to PioneOS!";
                _label1.Text = "このプロセスでは PioneOS  を設定するお手伝いをします。\n続けるには「次へ」をクリックしてください。";
                _thum.File = "/usr/share/pioneos/oobe/Assets/pioneos.png";
                _infomationbar.Visible = true;
                // アイコン設定
                _live_icon.IconName = "face-smile";
                _live_icon.PixelSize = 48;
                _live_icon.Visible = true;
                // 
                _pionever.Visible = true;
                _bypass_network++;
                if (_bypass_network == 7){
                    _button2.Visible = true;
                    _button2.Label = "bypass";
                    _button1.Label = "次へ＞";
                    _button2.Opacity = 0.5;
                }
                else{
                    _button2.Label = "＜戻る";
                    _button1.Label = "次へ＞";
                    _button2.Visible = false;           
                }
            }
            else if (_state == 1){
                _label2.Text = "１．インターネットへの接続";
                _button1.Visible = _button2.Visible = true;
                _navigationbar.Visible = true;
                _thum.Visible = true;
                eulaview.Visible = false;
                windoweulaview.Visible = false;
                _infomationbar.Visible = false;
                _label1.Text = "PioneOS のセットアップにはインターネット接続が必要です。\n無線 LAN の場合は画面左下のアイコンをクリックして接続先を選択します。\n有線 LAN の場合は接続してから左下のアイコンを確認します。";
                _button1.Label = "接続を確認＞";
                _button2.Label = "＜戻る";
                _thum.File = "/usr/share/pioneos/oobe/Assets/network.png";
                _pionever.Visible = false;
                // アイコン設定
                _live_icon.IconName = "internet-archive";
                _live_icon.PixelSize = 48;
                _live_icon.Visible = true;
                // 
            }
            else if (_state == 2){
                _button1.Visible = _button2.Visible = false;
                _label1.Text = "インターネット接続を確認中です。\nしばらくお待ちください。";
                await Task.Delay(50);
                try{
                    var connectcheck = new HttpClient();
                    await connectcheck.GetAsync("http://ospio.net");
                    connectcheck.Dispose();
                    _state++;
                }
                catch{
                    Dialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "インターネット接続が確認できませんでした。\n接続を確認してから再度お試しください。");
                    dialog.Run();
                    dialog.Destroy();
                    _state = 1;
                }
                await setAsync(sender, a);
            }
            else if (_state == 3){
                _button1.Visible = _button2.Visible = true;
                _label2.Text = "２．エンドユーザー ライセンスの確認";
                _navigationbar.Visible = false;
                toggleswitch.Visible = false;
                _label1.Text = "エンドユーザー ライセンスを読んで同意してください。\n同意しない場合は、PioneOS をご利用できません。";
                _thum.Visible = false;
                eulaview.Visible = true;
                _button1.Label = "同意＞";
                _button2.Label = "＜拒否";
                windoweulaview.Visible = true;
                // アイコン設定
                _live_icon.IconName = "notes";
                _live_icon.PixelSize = 48;
                _live_icon.Visible = true;
                // 
                try{
                    if (eulaviewed == 0){
                    var connectcheck = new HttpClient();
                    var eulaget = await connectcheck.GetAsync("http://ospio.net/eula.txt");
                    if (eulaget.IsSuccessStatusCode == false){
                        throw new Exception("EULA の取得に失敗しました。");
                    }
                    connectcheck.Dispose();
                    string eulakekka = await eulaget.Content.ReadAsStringAsync();
                    var tagLarge = new TextTag("large"){SizePoints = 17,Weight = Pango.Weight.Bold};
                    eulaview.Buffer.TagTable.Add(tagLarge);
                //eulakekka の行数を取得
                    var iter = eulaview.Buffer.EndIter;
                    var lines = eulakekka.Split('\n');
                
                   foreach (var line in eulakekka.Split('\n'))
                    {
                     if (line.StartsWith("!")){
                          string lineremove = line.Replace("!", "");
                          eulaview.Buffer.InsertWithTags(ref iter, lineremove + "\n", tagLarge);
                        }
                     else{
                          eulaview.Buffer.Insert(ref iter, line + "\n");
                      }
                      eulaviewed = 1;
                 }
                //行数分ループ
                }
                _navigationbar.Visible = true;
                }
                
                catch (Exception e){
                    Dialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "インターネット接続が確認できませんでした。接続を確認してから再度お試しください。\n\nエラー："+ e.Message);
                    dialog.Run();
                    dialog.Destroy();
                    _state = 1;
                    await setAsync(sender, a);
                }
            }
            else if (_state == 4){
                _label2.Text = "３．Windows とのデュアルブート環境向けの設定";
                _button1.Visible = true;
                _button2.Visible = true;
                toggleswitch.Visible = true;
                _progresscircle.Visible = false;
                toggleswitch.Label = "デュアルブート向け設定を適用する";
                _thum.Visible = false;
                eulaview.Visible = false;
                windoweulaview.Visible = false;
                packagelistbox.Visible = false;
                _label1.Text = "上のチェックを入れると、PioneOS と Windows をデュアルブートする環境向けに以下の設定を適用します。\n1. 時刻ズレを修正するためにシステム時刻をローカル時刻へセット\n2.Windows で再起動するアイコンを作成";
                _button1.Label = "次へ＞";
                _button2.Label = "＜戻る";
                _thum.PixelSize = 196;
                // アイコン設定
                _live_icon.IconName = "distributor-logo-windows";
                _live_icon.PixelSize = 48;
                _live_icon.Visible = true;
                // 
               if (windowstimesync == true){
                   toggleswitch.Active = true;
               }
              else{
                   toggleswitch.Active = false;
              }
            }
            else if(_state == 5){
                _label2.Text = "４．ソフトウェアの選択";
                _button1.Visible = true;
                _button2.Visible = true;
                toggleswitch.Visible = false;
                _thum.Visible = false;
                _progresscircle.Visible = true;
                eulaview.Visible = false;
                windoweulaview.Visible = false;
                _button1.Label = "次へ＞";
                _button2.Label = "＜戻る";
                _label1.Text = "ここでデフォルトで使用するソフトウェアを選択することができます。\nなお、ここに表示されるソフトウェアは後からでも手動でインストールできます。";
                // アイコン設定
                _live_icon.IconName = "downloader";
                _live_icon.PixelSize = 48;
                _live_icon.Visible = true;
                // インストールするパッケージィの設定
                if (packagelisted == false)
                {
                    texteditorcombo.AppendText("nano&vim");
                    texteditorcombo.Active = 0;
                    //Console.WriteLine("texteditorcombo is alive!");
                    var connectcheck2 = new HttpClient();
                    connectcheck2.Timeout = TimeSpan.FromSeconds(10);
                    var texteditorlistget = await connectcheck2.GetAsync("https://store.ospio.net/texteditorlist.xml");
                    if (texteditorlistget.IsSuccessStatusCode == true){
                        string texteditorlist = await texteditorlistget.Content.ReadAsStringAsync();
                        XDocument texteditorlistxml = XDocument.Parse(texteditorlist);
                        texteditorlistroot = texteditorlistxml.Element("data");
                        foreach (XElement textitem in texteditorlistroot.Elements("item"))
                        {
                            Console.WriteLine(textitem.Element("id").Value);
                            texteditorcombo.AppendText(textitem.Element("name").Value);
                        }
                    }
                    connectcheck2.Dispose();
                    packagelisted = true;
                }
                _progresscircle.Visible = false;
                texteditorcombo.Visible = true;
                packagelistbox.Visible = true;
            }
            else if(_state == 6){
                _label1.Text = "PioneOS を快適にご利用いただくための最終処理を行っています。\n自動で再起動するまでこのまましばらくお待ちください！";
                _button1.Visible = false;
                toggleswitch.Visible = false;
                _button2.Visible = false;
                _progress.Visible = true;
                packagelistbox.Visible = false;          
                _navigationbar.Visible = false;
                _thum.Visible = true;
                // アイコン設定
                _live_icon.IconName = "checkmark";
                _live_icon.PixelSize = 48;
                _live_icon.Visible = true;
                // 
                _label2.Text = "これで、初期設定が完了しました！";
                var image = new Gdk.Pixbuf("/usr/share/pioneos/oobe/Assets/pioneos.png");
                _thum.Pixbuf = image;
                await Task.Run(() => StartProgressBarAnimation(_progress));
                //まずは apt-get update だな
                var processInfo3 = new ProcessStartInfo
                {
                    FileName = "sudo",
                    UseShellExecute = false,
                    Arguments = "/usr/share/pioneos/oobe/apt-get.sh",
                    RedirectStandardOutput = true
                };
                Process.Start(processInfo3);
                //パッケージのインストール
                if(texteditorcombo.Active == 0)
                {
                    Console.WriteLine("テキストエディタはデフォルトのままです");
                }
                else
                {
                    // 選択された ID からアイテムを取得し、install をインストール
                    XElement installertext = texteditorlistroot.Elements("item").FirstOrDefault(item => (string)item.Element("id") == texteditorcombo.Active.ToString());
                    if (installertext != null)
                    {
                        string packageinstall = installertext.Element("install").Value;
                        string packageuninstall = installertext.Element("uninstall").Value;
                        //インストール開始
                        if(installertext.Element("howto").Value == "apt")
                        {
                            await Task.Delay(50);
                            var processInfo_textedit = new ProcessStartInfo
                            {
                                FileName = "sudo",
                                UseShellExecute = true,
                                Arguments = "/usr/share/pioneos/oobe/instremo.sh" + " " + packageinstall + " " + packageuninstall
                            };
                            Process.Start(processInfo_textedit);
                            Console.WriteLine(packageinstall + "をインストールしました。");
                        }
                    }
                }
            //時間同期など
            if (windowstimesync == true){
                Console.WriteLine("windowstimesync.sh を実行します。");
                await Task.Delay(50);
                var processInfo_windowstimesync = new ProcessStartInfo
                {
                    FileName = "sudo",
                    UseShellExecute = false,
                    Arguments = "/usr/share/pioneos/oobe/windowstimesync.sh",
                    RedirectStandardOutput = true,
                };
                Process.Start(processInfo_windowstimesync);
            }
            //最終処理、ここでサヨナラ
            Console.WriteLine("fini2.sh を実行します。");
            var process2Info = new ProcessStartInfo
            {
                FileName = "/usr/share/pioneos/oobe/fini2.sh",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            Console.WriteLine("fini.sh を実行します。");
            Process.Start(process2Info);
            await Task.Delay(50);
            var processInfo = new ProcessStartInfo
            {
                FileName = "sudo",
                UseShellExecute = false,
                Arguments = "/usr/share/pioneos/oobe/fini.sh",
                RedirectStandardOutput = true
            };
            Process.Start(processInfo);
            }
            static void StartProgressBarAnimation(ProgressBar _progress)
            {
                int animationInterval = 60;
                Timer? timer = null;
                bool increasing = true;
                timer = new Timer((state) =>
                {
                    double newFraction;
                    if (increasing)
                    {
                        newFraction = _progress.Fraction + 0.012;
                    }
                    else
                    {
                        newFraction = _progress.Fraction - 0.012;
                    }
                    if (newFraction >= 1.0 || newFraction <= 0.0)
                    {
                        newFraction = 0.0;
                    }
                    Application.Invoke((sender, e) =>
                    {
                        _progress.Fraction = newFraction;
                    });
                }, null, 0, animationInterval);
            }
        }
        //Gtk から該当のハンドルを受け取ったら
        private void settexteditor(object sender, EventArgs a)
        {
            if(texteditorcombo.Active != 0)
            {
                Console.WriteLine(texteditorcombo.Active + "番のテキストエディタが選択されました");
                XElement installertext = texteditorlistroot.Elements("item").FirstOrDefault(item => (string)item.Element("id") == texteditorcombo.Active.ToString());
                texteditoricon.IconName = installertext.Element("icon").Value;
            }
            else
            {
                texteditoricon.IconName = "mousepad";
            }
            

        }
    }
}