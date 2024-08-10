//using System;
using System.Diagnostics;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
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
        [UI] private ProgressBar _progress = null;
        [UI] private Box maingtkbox = null;
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
        private string eulakekka = "";
        private int _bypass_network = 0;
        private int eulaviewed = 0;
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
                var cssdata = "._navi_install {    background-color: #E0E0E0;       padding: 10px 10px 10px 10px;}._navigationbar{    background-color: #E0E0E0;    padding: 5px 5px 5px 5px}";
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
                _live_icon.Visible = false;
                _window_info.Visible = false;
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
            await Task.Delay(5);
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
            await Task.Delay(5);
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
            await Task.Delay(5);
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
            await Task.Delay(5);
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
                _label1.Text = "このプロセスでは PioneOS  を設定するお手伝いをします。\n続けるには「次へ」をクリックしてください。";
                _label2.Text = "Welcome to PioneOS!";
                _thum.File = "/usr/share/pioneos/oobe/Assets/pioneos.png";
                _infomationbar.Visible = true;
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
                _button1.Visible = true;
                _button2.Visible = true;
                _navigationbar.Visible = true;
                _thum.Visible = true;
                eulaview.Visible = false;
                windoweulaview.Visible = false;
                _infomationbar.Visible = false;
                _label1.Text = "PioneOS のセットアップにはインターネット接続が必要です。\n無線 LAN の場合は画面左下のアイコンをクリックして接続先を選択します。\n有線 LAN の場合は接続してから左下のアイコンを確認します。";
                _label2.Text = "１．インターネットへの接続";
                _button1.Label = "接続を確認＞";
                _button2.Label = "＜戻る";
                _thum.File = "/usr/share/pioneos/oobe/Assets/network.png";
                _pionever.Visible = false;
            }
            else if (_state == 2){
                _navigationbar.Visible = false;
                _thum.File = "/usr/share/pioneos/oobe/Assets/internet.png";
                _label1.Text = "インターネット接続を確認中です。\nしばらくお待ちください。";
                await Task.Delay(100);
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
                _navigationbar.Visible = false;
                toggleswitch.Visible = false;
                _label1.Text = "エンドユーザー ライセンスを読んで同意してください。\n同意しない場合は、PioneOS をご利用できません。";
                _label2.Text = "２．エンドユーザー ライセンスの確認";
                _thum.Visible = false;
                eulaview.Visible = true;
                _button1.Label = "同意＞";
                _button2.Label = "＜拒否";
                windoweulaview.Visible = true;
                try{
                    var connectcheck = new HttpClient();
                    var eulaget = await connectcheck.GetAsync("http://ospio.net/eula.txt");
                    if (eulaget.IsSuccessStatusCode == false){
                        throw new Exception("EULA の取得に失敗しました。");
                    }
                    string eulakekka = await eulaget.Content.ReadAsStringAsync();
                    var tagLarge = new TextTag("large"){SizePoints = 17,Weight = Pango.Weight.Bold};
                    eulaview.Buffer.TagTable.Add(tagLarge);
                //eulakekka の行数を取得
                    var iter = eulaview.Buffer.EndIter;
                    var lines = eulakekka.Split('\n');
                if (eulaviewed == 0){
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
                _button1.Visible = true;
                _button2.Visible = true;
                toggleswitch.Visible = true;
                toggleswitch.Label = "デュアルブート向け設定を適用する";
                _thum.Visible = true;
                eulaview.Visible = false;
                windoweulaview.Visible = false;
                _label1.Text = "上のチェックを入れると、PioneOS と Windows をデュアルブートする環境向けに以下の設定を適用します。\n1. 時刻ズレを修正するためにシステム時刻をローカル時刻へセット\n2.Windows で再起動するアイコンを作成";
                _label2.Text = "３．Windows とデュアルブートする環境向けの設定を使用しますか？";
                _button1.Label = "次へ＞";
                _button2.Label = "＜戻る";
                _thum.IconName = "distributor-logo-windows";
                _thum.PixelSize = 256;
               if (windowstimesync == true){
                   toggleswitch.Active = true;
               }
              else{
                   toggleswitch.Active = false;
              }
            }
            else if(_state == 5){
                _button1.Visible = false;
                toggleswitch.Visible = false;
                _button2.Visible = false;
                _progress.Visible = true;
                _navigationbar.Visible = false;
                _thum.Visible = true;
                _label1.Text = "これで、初期設定が完了しました！\nPioneOS を快適にご利用いただくために\n最終処理を行っていますので\n再起動するまで絶対にデバイスに触れないでください！";
                _label2.Text = "自動で再起動するまでお待ちください！";
                var image = new Gdk.Pixbuf("/usr/share/pioneos/oobe/Assets/pioneos.png");
                _thum.Pixbuf = image;
                await Task.Run(() => StartProgressBarAnimation(_progress));
            
            if (windowstimesync == true){
                Console.WriteLine("windowstimesync.sh を実行します。");
                var processInfo_windowstimesync = new ProcessStartInfo
                {
                    FileName = "sudo",
                    UseShellExecute = false,
                    Arguments = "/usr/share/pioneos/oobe/windowstimesync.sh",
                    RedirectStandardOutput = true,
                };
                Process.Start(processInfo_windowstimesync);
            }
            Console.WriteLine("fini2.sh を実行します。");
            var process2Info = new ProcessStartInfo
            {
                FileName = "/usr/share/pioneos/oobe/fini2.sh",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            Console.WriteLine("fini.sh を実行します。");
            Process.Start(process2Info);
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
    }
}