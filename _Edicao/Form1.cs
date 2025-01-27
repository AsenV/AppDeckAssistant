using MetroFramework;
using MetroFramework.Components;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Net; // Para WebClient
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using static Humanizer.On;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices.ComTypes;

namespace AppDeckAssistant
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        private string programFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
        private string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Asen Lab", "ASL AppDeck Assistant");
        private string caminhoArquivoAtual = null;
        private string ultimoArquivo = Properties.Settings.Default.UltimoArquivo;
        private string ultimoTema, ultimoEstilo;
        //private readonly bool _isDarkMode = DarkModeHelper.IsDarkModeEnabled();
        private int minWidth, minHeight;
        private Color HeaderBackColor, ThemeBackColor, ThemeForeColor, ExpiringColor15, ExpiringColor6, ExpiredColor, StyleForeColor;
        private MetroStyleManager mStyleManager = new();
        private ConfigsForm configs = new();
        private SobreForm sobre = new();

        private BannerForm bannerf;
        private string nomeJogo;
        private ListViewItem itemSelecionado;
        private string imagemSelecionada;

        private string jogoId;
        private int posicaoJogo;
        private string caminhoImagem;

        public string NomeDoProjeto { get; } = "ASL AppDeck Assistant";
        public string VersaoDoArquivo { get; } = ObterVersaoDoArquivo();
        public string NomeDaEmpresa { get; } = "Asen Lab Corporation";

        // Mapeamento de jogos e seus respectivos IDs
        private Dictionary<string, string> jogosSteam = new Dictionary<string, string> { };

        private Dictionary<string, string> CarregarJogosSteamDeArquivo(string caminhoArquivo)
        {
            Dictionary<string, string> jogosSteam = new Dictionary<string, string>();

            try
            {
                if (!File.Exists(caminhoArquivo))
                {
                    // Cria o arquivo vazio com um cabeçalho opcional (opcionalmente você pode deixar o arquivo vazio)
                    using (StreamWriter writer = new StreamWriter(caminhoArquivo))
                    {
                        writer.WriteLine("ID\t\tNome do Jogo"); // Cabeçalho opcional
                    }

                    // Retorna um dicionário vazio
                    return jogosSteam;
                }

                // Lê todas as linhas do arquivo
                string[] linhas = File.ReadAllLines(caminhoArquivo);

                foreach (string linha in linhas)
                {
                    // Ignora linhas vazias
                    if (string.IsNullOrWhiteSpace(linha)) continue;

                    // Divide a linha em ID e nome usando dois caracteres de tabulação (\t\t)
                    string[] partes = linha.Split(new string[] { "\t\t" }, StringSplitOptions.None);

                    if (partes.Length == 2)
                    {
                        string id = partes[0].Trim();
                        string nome = partes[1].Trim();

                        // Adiciona ao dicionário
                        jogosSteam[nome] = id;
                    }
                    else
                    {
                        MessageBox.Show($"Linha inválida no arquivo: {linha}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Erro ao carregar jogos Steam do arquivo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return jogosSteam;
        }

        public event EventHandler DarkModeChanged;
        private bool _isDarkMode = DarkModeHelper.IsDarkModeEnabled();
        public bool DarkMode
        {
            get { return _isDarkMode; }
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    OnDarkModeChanged();
                }
            }
        }

        protected virtual void OnDarkModeChanged()
        {
            DarkModeChanged?.Invoke(this, EventArgs.Empty);
            UpdateTheme();
        }

        private MetroThemeStyle _metroTheme;
        public MetroThemeStyle MetroTheme
        {
            get { return mStyleManager.Theme; }
            set
            {
                if (_metroTheme != value)
                {
                    mStyleManager.Theme = value;
                    _metroTheme = value;
                    mStyleManager.Update();
                }
            }
        }

        private MetroColorStyle _metroStyle;
        public MetroColorStyle MetroStyle
        {
            get { return mStyleManager.Style; }
            set
            {
                if (_metroStyle != value)
                {
                    mStyleManager.Style = value;
                    _metroStyle = value;
                    mStyleManager.Update();
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            configs.cbThemeChanged += Configs_ThemeChanged;
            configs.cbStyleChanged += Configs_StyleChanged;
            mStyleManager.Owner = this;
            this.StyleManager = mStyleManager;
            Application.AddMessageFilter(new ToolbarMessageFilter(toolBar1)); // Desfocar ao clicar fora
            //Application.AddMessageFilter(new DropDownMessageFilter(comboCheckBox1)); // Desfocar ao clicar fora
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadUserSettings();
            UpdateThisAppearence();
            InitializeToolBar();
            LoadPreviousDocument();
            InicializarJogosSteam();
        }

        private void InicializarJogosSteam()
        {
            if (!string.IsNullOrEmpty(caminhoArquivoAtual))//&& File.Exists(ultimoArquivo))
            {

                if (!Directory.Exists(Path.Combine(caminhoArquivoAtual, "@Resources")))
                {
                    return;
                }
                string caminhoArquivoSteamIds = Path.Combine(caminhoArquivoAtual, "@Resources", "User", "steamids.txt");
                jogosSteam = CarregarJogosSteamDeArquivo(caminhoArquivoSteamIds);

                if (jogosSteam.Count > 0)
                {
                    //MessageBox.Show("Jogos Steam carregados com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private void LoadPreviousDocument()
        {
            if (!string.IsNullOrEmpty(ultimoArquivo))//&& File.Exists(ultimoArquivo))
            {
                if (!Directory.Exists(Path.Combine(ultimoArquivo, "@Resources")))
                {
                    LimparLista();
                    return;
                }
                // Se existir, carregue o último arquivo
                caminhoArquivoAtual = ultimoArquivo;
                CarregarListaDeArquivo(ultimoArquivo);
                UpdateTitle();
            }
            else
            {
                LimparLista();
                // Caso contrário, inicie um novo arquivo ou tome outras ações necessárias
                // ...
            }
            UpdateTitle();
        }

        private void LoadUserSettings()
        {
            string dataFolder = Path.Combine(appData, "Data");
            string userSettingsPath = Path.Combine(dataFolder, "user.xml");

            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            if (Directory.Exists(dataFolder))
            {
                if (File.Exists(userSettingsPath))
                {
                    var xml = XDocument.Load(userSettingsPath);
                    var settingsElement = xml.Element("Settings");

                    if (settingsElement != null)
                    {
                        var ultimoTemaElement = settingsElement.Element("UltimoTema");
                        var ultimoEstiloElement = settingsElement.Element("UltimoEstilo");

                        if (ultimoTemaElement != null && ultimoEstiloElement != null)
                        {
                            ultimoTema = ultimoTemaElement.Value;
                            ultimoEstilo = ultimoEstiloElement.Value;
                        }
                    }
                }
            }
        }

        private void SaveUserSettings()
        {
            string dataFolder = Path.Combine(appData, "Data");
            string userSettingsPath = Path.Combine(dataFolder, "user.xml");
            var xml = new XDocument();

            if (File.Exists(userSettingsPath))
            {
                xml = XDocument.Load(userSettingsPath);
            }
            else
            {
                xml.Add(new XElement("Settings"));
            }

            var settingsElement = xml.Element("Settings");

            if (settingsElement == null)
            {
                settingsElement = new XElement("Settings");
                xml.Add(settingsElement);
            }

            settingsElement.SetElementValue("UltimoTema", MetroTheme.ToString());
            settingsElement.SetElementValue("UltimoEstilo", MetroStyle.ToString());

            xml.Save(userSettingsPath);
        }

        public void Configs_ThemeChanged(object sender, EventArgs e)
        {
            MetroTheme = configs.SelectedTheme;
            UpdateTheme();
        }

        public void Configs_StyleChanged(object sender, EventArgs e)
        {
            MetroStyle = configs.SelectedStyle;
            UpdateTheme();
        }

        private void UpdateThisAppearence()
        {
            minWidth = 350; //ClientSize.Width / 2 - (minWidth % 1); // Largura minima da janela
            minHeight = 530; //ClientSize.Height / 2 - (minHeight % 1); // Altura minima da janela
            listGames.Columns[0].Width = listGames.Width - 25;

            MetroThemeStyle temaConvertido;
            MetroColorStyle estiloConvertido;
            MetroTheme = Enum.TryParse(ultimoTema, out temaConvertido) ? temaConvertido : MetroThemeStyle.Dark;
            MetroStyle = Enum.TryParse(ultimoEstilo, out estiloConvertido) ? estiloConvertido : MetroColorStyle.Orange;

            UpdateTheme();
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            if (caminhoArquivoAtual != null)
            {
                if (!Directory.Exists(Path.Combine(caminhoArquivoAtual, "@Resources")))
                {
                    return;
                }

                string nomeArquivo = Path.GetFileName(caminhoArquivoAtual);
                string titleName = $"{NomeDoProjeto} {VersaoDoArquivo} ({nomeArquivo})";
                this.Text = titleName;
                windowBar1.Title = titleName;

                if (picBanner.Image != null)
                {
                    picBanner.Image.Dispose();
                    imagemSelecionada = null;
                    picBanner.Image = null;
                }
                imagemSelecionada = Path.Combine(caminhoArquivoAtual, "@Resources", "User", "Banners", $"banner{posicaoJogo}.jpg");
                if (File.Exists(imagemSelecionada))
                {
                    // Faz uma cópia temporária da imagem para evitar bloqueio no arquivo original
                    string caminhoTempPreview = Path.Combine(Path.GetTempPath(), "preview_temp.jpg");
                    File.Copy(imagemSelecionada, caminhoTempPreview, true);

                    // Carrega a imagem copiada no PictureBox
                    using (Image imagemPreview = Image.FromFile(caminhoTempPreview))
                    {
                        picBanner.Image = new Bitmap(imagemPreview); // Cria um novo Bitmap para liberar o arquivo
                    }

                    // Libera o arquivo temporário, se necessário, em outro ponto do código
                }
                else
                {
                    picBanner.Image = null; // Limpa o PictureBox se a imagem não existir
                }
            }
            else
            {
                this.Text = $"{NomeDoProjeto} {VersaoDoArquivo}";
                windowBar1.Title = $"{NomeDoProjeto} {VersaoDoArquivo}";
            }
            windowBar1.Width = +1;
            windowBar1.Width = -1;
            UpdateInfo();
        }

        private void UpdateTheme()
        {
            HeaderBackColor = _isDarkMode ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
            ThemeBackColor = _isDarkMode ? Color.FromArgb(27, 27, 27) : Color.FromArgb(255, 255, 255);
            ThemeForeColor = _isDarkMode ? Color.FromArgb(190, 190, 190) : Color.FromArgb(17, 17, 17);
            StyleForeColor = _isDarkMode ? MetroColorStyleToColor(MetroStyle) : ThemeForeColor;

            ExpiringColor15 = _isDarkMode ? Color.FromArgb(115, 99, 18) : Color.FromArgb(255, 242, 176);
            ExpiringColor6 = _isDarkMode ? Color.FromArgb(115, 65, 18) : Color.FromArgb(255, 193, 138);
            ExpiredColor = _isDarkMode ? Color.FromArgb(115, 18, 18) : Color.FromArgb(255, 117, 117);

            picBanner.SizeMode = PictureBoxSizeMode.Zoom;

            listGames.BackColor = ThemeBackColor;
            listGames.ForeColor = ThemeForeColor;
            label1.ForeColor = ThemeForeColor;

            ApplyButtomTheme(btnImport, true);
            ApplyButtomTheme(btnRemove, true);

            //txtAdress.BackColor = ThemeBackColor;
            //txtAdress.ForeColor = ThemeForeColor;

            mStyleManager.Theme = MetroTheme;
            configs.MetroTheme = MetroTheme;
            windowBar1.MetroTheme = MetroTheme;
            toolBar1.MetroTheme = MetroTheme;

            this.DarkMode = (MetroTheme == MetroThemeStyle.Dark);
            configs.DarkMode = (MetroTheme == MetroThemeStyle.Dark);
            sobre.DarkMode = (MetroTheme == MetroThemeStyle.Dark);
            windowBar1.DarkMode = (MetroTheme == MetroThemeStyle.Dark);
            toolBar1.DarkMode = (MetroTheme == MetroThemeStyle.Dark);

            mStyleManager.Style = MetroStyle;
            configs.MetroStyle = MetroStyle;
            windowBar1.MetroStyle = MetroStyle;
            toolBar1.MetroStyle = MetroStyle;

            labelInfo.Text = "";
        }

        private void ApplyButtomTheme(CustomButton button, bool highlight)
        {
            button.ButtonHighlight = highlight;
            button.ButtonForeColor = ThemeForeColor;
            button.ButtonBackColor = ThemeBackColor;
            button.ButtonBorderColor = ThemeForeColor;
            button.ButtonHighlightBackColor = ThemeBackColor;
            button.ButtonHighlightForeColor = ThemeForeColor;
            button.ButtonBorderHighlightColor = StyleForeColor;
        }

        private void InitializeToolBar()
        {
            // Adicione botões à barra de ferramentas (use "-" como separador)
            toolBar1.AddButton("File", new List<string> { "Find Skin..", "-", "Close" }, index =>
            {
                if (index == 0)
                {
                    AbrirArquivo();
                }
                else if (index == 2)
                {
                    Application.Exit();
                }
            });

            toolBar1.AddButton("About", new List<string> { "Customize", "-", "About" }, index =>
            {
                if (index == 0)
                {
                    configs.UpdateUserSettings(MetroTheme, MetroStyle);
                    configs.ShowDialog();
                }
                else if (index == 2)
                {
                    sobre.UpdateUserSettings(MetroTheme, MetroStyle);
                    sobre.ShowDialog();
                }
            });

            toolBar1.AddButton("Tools", new List<string> { "Auto Search IDs"}, index =>
            {
                if (index == 0)
                {
                    SyncSteamBanners();
                }
            });
        }

        protected override void OnResizeBegin(EventArgs e)
        {
            base.OnResizeBegin(e);
            MinimumSize = new Size(minWidth, minHeight);
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            if (Width < minWidth) Width = minWidth;
            if (Height < minHeight) Height = minHeight;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            // Desabilitar redimensionamento estando maximizado
            this.Resizable = WindowState != FormWindowState.Maximized;
            listGames.Columns[0].Width = listGames.Width - 25;
            //listGames.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.UltimoArquivo = caminhoArquivoAtual;
            Properties.Settings.Default.Save();
            SaveUserSettings();
        }

        private static string ObterVersaoDoArquivo()
        {
            FileVersionInfo versaoArquivo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            return versaoArquivo.FileVersion;
        }
        
        private string ObterURLDeAtalho(string caminhoAtalho)
        {
            if (caminhoAtalho.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
            {
                return ObterURLDeLnk(caminhoAtalho); // Processa arquivos .lnk
            }
            else if (caminhoAtalho.EndsWith(".url", StringComparison.OrdinalIgnoreCase))
            {
                return ObterURLDeUrl(caminhoAtalho); // Processa arquivos .url
            }

            return string.Empty; // Caso não seja nem .lnk nem .url
        }

        // Obtém o URL de um atalho .lnk
        private string ObterURLDeLnk(string caminhoAtalho)
        {
            try
            {
                // Usa COM para resolver o alvo do atalho
                Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(shellType);
                dynamic lnk = shell.CreateShortcut(caminhoAtalho);

                string targetPath = lnk.TargetPath;
                Marshal.ReleaseComObject(lnk);
                Marshal.ReleaseComObject(shell);

                return targetPath; // Retorna o caminho/URL do atalho
            }
            catch
            {
                return string.Empty; // Retorna vazio se ocorrer algum erro
            }
        }

        // Obtém o URL de um atalho .url
        private string ObterURLDeUrl(string caminhoAtalho)
        {
            try
            {
                var lines = File.ReadAllLines(caminhoAtalho);
                foreach (string line in lines)
                {
                    if (line.StartsWith("URL=", StringComparison.OrdinalIgnoreCase))
                    {
                        return line.Substring(4); // Retorna o valor após "URL="
                    }
                }
            }
            catch
            {
                // Ignorar erros de leitura
            }

            return string.Empty; // Retorna vazio se o URL não for encontrado
        }

        private void UpdateInfo()
        {
            //txtAdress.Text = caminhoArquivoAtual;
        }

        private void LimparLista()
        {
            //dataGridViewProdutos.DataSource = null;
            caminhoArquivoAtual = null;
            ultimoArquivo = null;
            Properties.Settings.Default.UltimoArquivo = null;
            Properties.Settings.Default.Save();
            listGames.Items.Clear();
            if (picBanner.Image != null)
            {
                picBanner.Image.Dispose();
                imagemSelecionada = null;
                picBanner.Image = null;
            }
            UpdateTitle();
            UpdateInfo();
        }

        private void AbrirArquivo()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Title = "Select AppDeckPro skin folder";

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    caminhoArquivoAtual = dialog.FileName;
                    if (!Directory.Exists(Path.Combine(caminhoArquivoAtual, "@Resources")))
                    {
                        LimparLista();
                        MessageBox.Show("AppDeckPro skin folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    CarregarListaDeArquivo(caminhoArquivoAtual);
                    Properties.Settings.Default.UltimoArquivo = caminhoArquivoAtual;
                    Properties.Settings.Default.Save();
                    //txtAdress.Text = caminhoArquivoAtual;
                    listGames.Refresh();
                    UpdateTitle();
                }
            }
        }

        private void CarregarListaDeArquivo(string caminhoArquivo)
        {
            string caminhoArquivoFiles = Path.Combine(caminhoArquivo, "@Resources", "User", "Files");

            try
            {
                listGames.Items.Clear();

                if (!Directory.Exists(caminhoArquivoFiles))
                {
                    MessageBox.Show("The specified path doesn't exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] arquivos = Directory.GetFiles(caminhoArquivoFiles, "*.*", SearchOption.TopDirectoryOnly)
                                             .Where(f => f.EndsWith(".lnk") || f.EndsWith(".url"))
                                             .ToArray();

                foreach (string arquivo in arquivos)
                {
                    string nomeArquivoSemExtensao = Path.GetFileNameWithoutExtension(arquivo);
                    string url = ObterURLDeAtalho(arquivo); // Obtém o URL do atalho

                    // Adiciona o item à lista e armazena o URL no Tag
                    ListViewItem item = new ListViewItem(nomeArquivoSemExtensao)
                    {
                        Tag = url // URL armazenado no Tag
                    };

                    listGames.Items.Add(item);
                }

                if (listGames.Items.Count > 0)
                {
                    listGames.Items[0].Selected = true;
                    itemSelecionado = listGames.Items[0];
                    listGames.Select();
                }

                posicaoJogo = listGames.Items.IndexOf(itemSelecionado) + 1;
                SyncLinks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading the list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImport_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                string caminhoBanners = Path.Combine(caminhoArquivoAtual, "@Resources", "User", "Banners");
                string caminhoArquivos = Path.Combine(caminhoArquivoAtual, "@Resources", "User", "Files");
                string caminhoTemp = Path.Combine(caminhoBanners, "@Reload");

                if (!Directory.Exists(Path.Combine(caminhoArquivoAtual, "@Resources")))
                {
                    return;
                }

                if (!Directory.Exists(caminhoTemp))
                {
                    Directory.CreateDirectory(caminhoTemp);
                }
                
                // Copiar os banners para a pasta @Reload, renomeando-os conforme a posição na listGames
                for (int i = 0; i < listGames.Items.Count; i++)
                {
                    ListViewItem item = listGames.Items[i];
                    string nomeJogo = item.Text; // Nome do jogo
                    string caminhoImagemOriginal = Path.Combine(caminhoBanners, $"banner{i + 1}.jpg");
                    string caminhoImagemTemp = Path.Combine(caminhoTemp, $"{nomeJogo}.jpg");

                    // Verifica se a imagem existe e copia para a pasta @Reload
                    if (File.Exists(caminhoImagemOriginal))
                    {
                        File.Copy(caminhoImagemOriginal, caminhoImagemTemp, true);
                    }
                }

                // Exclui os banners da pasta Banners após cópias para @Reload
                var arquivosBanners = Directory.GetFiles(caminhoBanners, "banner*.jpg");
                foreach (var arquivo in arquivosBanners)
                {
                    if (File.Exists(arquivo))
                    {
                        File.Delete(arquivo);
                    }
                }

                // Exibir o diálogo para o usuário escolher arquivos .lnk e .url
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Multiselect = true,
                    Filter = "Atalhos (*.lnk;*.url)|*.lnk;*.url"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Cria a pasta 'files' se não existir
                    if (!Directory.Exists(caminhoArquivos))
                    {
                        Directory.CreateDirectory(caminhoArquivos);
                    }

                    // Copia os atalhos selecionados para a pasta 'files'
                    foreach (var arquivo in openFileDialog.FileNames)
                    {
                        string destino = Path.Combine(caminhoArquivos, Path.GetFileName(arquivo));
                        File.Copy(arquivo, destino, true);
                    }

                    // Atualiza a lista listGames com os novos arquivos
                    CarregarListaDeArquivo(caminhoArquivoAtual);

                    //MessageBox.Show("Importação e atualização concluídas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Renomeia os banners na pasta @Reload conforme a ordem na listGames
                for (int i = 0; i < listGames.Items.Count; i++)
                {
                    ListViewItem item = listGames.Items[i];
                    string nomeJogo = item.Text; // Nome do jogo
                    string caminhoImagemTemp = Path.Combine(caminhoTemp, $"{nomeJogo}.jpg");
                    string caminhoImagemFinal = Path.Combine(caminhoBanners, $"banner{i + 1}.jpg");

                    // Renomeia e move o banner de volta para a pasta Banners
                    if (File.Exists(caminhoImagemTemp))
                    {
                        File.Move(caminhoImagemTemp, caminhoImagemFinal);
                    }
                }

                // Aguarda um pequeno intervalo para garantir que os arquivos sejam liberados
                System.Threading.Thread.Sleep(500);

                // Verifica se a pasta ainda contém arquivos antes de tentar deletar
                if (Directory.Exists(caminhoTemp) && Directory.GetFiles(caminhoTemp).Length == 0)
                {
                    Directory.Delete(caminhoTemp, true); // Deleta a pasta, incluindo subpastas e arquivos (se houver)
                }

                SyncLinks();

                if (File.Exists(imagemSelecionada))
                {
                    // Faz uma cópia temporária da imagem para evitar bloqueio no arquivo original
                    string caminhoTempPreview = Path.Combine(Path.GetTempPath(), "preview_temp.jpg");
                    File.Copy(imagemSelecionada, caminhoTempPreview, true);

                    // Carrega a imagem copiada no PictureBox
                    using (Image imagemPreview = Image.FromFile(caminhoTempPreview))
                    {
                        picBanner.Image = new Bitmap(imagemPreview); // Cria um novo Bitmap para liberar o arquivo
                    }

                    // Libera o arquivo temporário, se necessário, em outro ponto do código
                }
                else
                {
                    picBanner.Image = null; // Limpa o PictureBox se a imagem não existir
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing files: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemove_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                string caminhoBanners = Path.Combine(caminhoArquivoAtual, "@Resources", "User", "Banners");
                string caminhoArquivos = Path.Combine(caminhoArquivoAtual, "@Resources", "User", "Files");
                string caminhoRemoved = Path.Combine(caminhoArquivoAtual, "@Resources", "User", ". (Removed)");
                string caminhoTemp = Path.Combine(caminhoBanners, "@Reload");

                if (!Directory.Exists(Path.Combine(caminhoArquivoAtual, "@Resources")))
                {
                    return;
                }

                if (!Directory.Exists(caminhoTemp))
                {
                    Directory.CreateDirectory(caminhoTemp);
                }

                // Copiar os banners para a pasta @Reload, renomeando-os conforme a posição na listGames
                for (int i = 0; i < listGames.Items.Count; i++)
                {
                    ListViewItem item = listGames.Items[i];
                    string nomeJogo = item.Text; // Nome do jogo
                    string caminhoImagemOriginal = Path.Combine(caminhoBanners, $"banner{i + 1}.jpg");
                    string caminhoImagemTemp = Path.Combine(caminhoTemp, $"{nomeJogo}.jpg");

                    // Verifica se a imagem existe e copia para a pasta @Reload
                    if (File.Exists(caminhoImagemOriginal))
                    {
                        File.Copy(caminhoImagemOriginal, caminhoImagemTemp, true);
                    }
                }

                // Exclui os banners da pasta Banners após cópias para @Reload
                var arquivosBanners = Directory.GetFiles(caminhoBanners, "banner*.jpg");
                foreach (var arquivo in arquivosBanners)
                {
                    if (File.Exists(arquivo))
                    {
                        File.Delete(arquivo);
                    }
                }

                // Confirmação de remoção
                DialogResult dialogResult = MessageBox.Show($"Are you sure you want to remove the game '{itemSelecionado.Text}'?", "Confirm Removing", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    // Caminho do banner
                    string caminhoBanner = Path.Combine(caminhoTemp, $"{nomeJogo}.jpg");

                    // Caminho dos atalhos .lnk e .url
                    string caminhoAtalhoLnk = Path.Combine(caminhoArquivos, $"{nomeJogo}.lnk");
                    string caminhoAtalhoUrl = Path.Combine(caminhoArquivos, $"{nomeJogo}.url");

                    // Cria a pasta . (Removed) se não existir
                    if (!Directory.Exists(caminhoRemoved))
                    {
                        Directory.CreateDirectory(caminhoRemoved);
                    }

                    // Mover o banner para a pasta . (Removed)
                    if (File.Exists(caminhoBanner))
                    {
                        string destinoBanner = Path.Combine(caminhoRemoved, $"{nomeJogo}.jpg");
                        File.Delete(destinoBanner);
                        File.Move(caminhoBanner, destinoBanner);
                    }

                    // Mover o atalho .lnk para a pasta . (Removed)
                    if (File.Exists(caminhoAtalhoLnk))
                    {
                        string destinoAtalhoLnk = Path.Combine(caminhoRemoved, $"{nomeJogo}.lnk");
                        File.Delete(destinoAtalhoLnk);
                        File.Move(caminhoAtalhoLnk, destinoAtalhoLnk);
                    }

                    // Mover o atalho .url para a pasta . (Removed)
                    if (File.Exists(caminhoAtalhoUrl))
                    {
                        string destinoAtalhoUrl = Path.Combine(caminhoRemoved, $"{nomeJogo}.url");
                        File.Delete(destinoAtalhoUrl);
                        File.Move(caminhoAtalhoUrl, destinoAtalhoUrl);
                    }

                    // Atualiza a lista listGames
                    CarregarListaDeArquivo(caminhoArquivoAtual);
                }

                // Renomeia os banners na pasta @Reload conforme a ordem na listGames
                for (int i = 0; i < listGames.Items.Count; i++)
                {
                    ListViewItem item = listGames.Items[i];
                    string nomeJogo = item.Text; // Nome do jogo
                    string caminhoImagemTemp = Path.Combine(caminhoTemp, $"{nomeJogo}.jpg");
                    string caminhoImagemFinal = Path.Combine(caminhoBanners, $"banner{i + 1}.jpg");

                    // Renomeia e move o banner de volta para a pasta Banners
                    if (File.Exists(caminhoImagemTemp))
                    {
                        File.Move(caminhoImagemTemp, caminhoImagemFinal);
                    }
                }

                // Aguarda um pequeno intervalo para garantir que os arquivos sejam liberados
                System.Threading.Thread.Sleep(500);

                // Verifica se a pasta ainda contém arquivos antes de tentar deletar
                if (Directory.Exists(caminhoTemp) && Directory.GetFiles(caminhoTemp).Length == 0)
                {
                    Directory.Delete(caminhoTemp, true); // Deleta a pasta, incluindo subpastas e arquivos (se houver)
                }

                SyncLinks();

                if (File.Exists(imagemSelecionada))
                {
                    // Faz uma cópia temporária da imagem para evitar bloqueio no arquivo original
                    string caminhoTempPreview = Path.Combine(Path.GetTempPath(), "preview_temp.jpg");
                    File.Copy(imagemSelecionada, caminhoTempPreview, true);

                    // Carrega a imagem copiada no PictureBox
                    using (Image imagemPreview = Image.FromFile(caminhoTempPreview))
                    {
                        picBanner.Image = new Bitmap(imagemPreview); // Cria um novo Bitmap para liberar o arquivo
                    }

                    // Libera o arquivo temporário, se necessário, em outro ponto do código
                }
                else
                {
                    picBanner.Image = null; // Limpa o PictureBox se a imagem não existir
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void picBanner_MouseDown(object sender, MouseEventArgs e)
        {

            // Exibe o MessageBox com dois botões: Sim (Yes) e Não (No)
            DialogResult result = MessageBox.Show(
                "Want to search a Steam ID?",  // Texto da mensagem
                "Confirmation",               // Título da caixa de mensagem
                MessageBoxButtons.YesNoCancel,        // Botões a serem exibidos
                MessageBoxIcon.Question         // Ícone exibido na caixa
            );

            // Verifica qual botão foi clicado
            if (result == DialogResult.Yes)
            {
                // Chama o método para a opção "Sim"

                ShowBannerForm(true);
            }
            else if (result == DialogResult.No)
            {
                // Chama o método para a opção "Não"

                ShowBannerForm(false);
            }
        }

        public Color MetroColorStyleToColor(MetroColorStyle metroColorStyle)
        {
            // Converte o MetroColorStyle para string
            string metroColorStyleName = metroColorStyle.ToString();

            // Retira o prefixo "Metro" e mapeia para a cor correspondente
            switch (metroColorStyleName)
            {
                case "Black":
                    return Color.Silver;
                case "White":
                    return Color.Silver;
                case "Silver":
                    return Color.Silver;
                case "Blue":
                    return Color.Cyan;
                case "Green":
                    return Color.PaleGreen;
                case "Lime":
                    return Color.Lime;
                case "Teal":
                    return Color.Teal;
                case "Orange":
                    return Color.Orange;
                case "Brown":
                    return Color.Brown;
                case "Pink":
                    return Color.Pink;
                case "Magenta":
                    return Color.Magenta;
                case "Purple":
                    return Color.Purple;
                case "Red":
                    return Color.Red;
                case "Yellow":
                    return Color.Yellow;
                default:
                    return Color.Silver; // Retorna Color.Empty se o estilo não for encontrado
            }
        }

        private void picBanner_MouseEnter(object sender, EventArgs e)
        {
            picBanner.BackColor = StyleForeColor; // Define o fundo da borda como laranja
            picBanner.Margin = new Padding(8); // Define a espessura da borda
        }

        private void picBanner_MouseLeave(object sender, EventArgs e)
        {
            picBanner.BackColor = Color.Transparent; // Remove a cor de fundo
            picBanner.Margin = new Padding(0); // Remove a borda
        }


        private void listGames_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                if (caminhoArquivoAtual != null)
                {
                    itemSelecionado = e.Item;
                    posicaoJogo = listGames.Items.IndexOf(itemSelecionado) + 1;
                    nomeJogo = itemSelecionado.Text;
                    if (picBanner.Image != null)
                    {
                        picBanner.Image.Dispose();
                        imagemSelecionada = null;
                        picBanner.Image = null;
                    }
                    imagemSelecionada = Path.Combine(caminhoArquivoAtual, "@Resources", "User", "Banners", $"banner{posicaoJogo}.jpg");
                    if (File.Exists(imagemSelecionada))
                    {
                        // Faz uma cópia temporária da imagem para evitar bloqueio no arquivo original
                        string caminhoTempPreview = Path.Combine(Path.GetTempPath(), "preview_temp.jpg");
                        File.Copy(imagemSelecionada, caminhoTempPreview, true);

                        // Carrega a imagem copiada no PictureBox
                        using (Image imagemPreview = Image.FromFile(caminhoTempPreview))
                        {
                            picBanner.Image = new Bitmap(imagemPreview); // Cria um novo Bitmap para liberar o arquivo
                        }

                        // Libera o arquivo temporário, se necessário, em outro ponto do código
                    }
                    else
                    {
                        picBanner.Image = null; // Limpa o PictureBox se a imagem não existir
                    }

                }
            }
        }

        private void SyncLinks()
        {
            try
            {
                string pathLinksInc = Path.Combine(caminhoArquivoAtual, "@Resources", "User", "links.inc"); // Caminho do arquivo
                List<string> urls = new List<string>(); // Lista para armazenar os URLs dos atalhos

                // Itera sobre os itens do MetroListView
                foreach (ListViewItem item in listGames.Items)
                {
                    // Obtém o URL associado ao atalho (supondo que esteja na Tag ou SubItem)
                    string url = item.Tag as string ?? item.SubItems[1]?.Text ?? string.Empty;

                    if (!string.IsNullOrEmpty(url))
                        urls.Add(url); // Adiciona à lista se o URL não for vazio
                }

                // Calcula o número de páginas necessário
                int totalLinks = urls.Count;
                int totalPages = (int)Math.Ceiling(totalLinks / 4.0);

                // Monta o conteúdo do arquivo links.inc
                List<string> fileLines = new List<string>();
                fileLines.Add("[Variables]");
                fileLines.Add($"pages={totalPages}");

                for (int i = 0; i < totalLinks; i++)
                {
                    fileLines.Add($"lnk{i + 1}={urls[i]}"); // Adiciona os URLs
                }

                // Preenche os links restantes com valores em branco, para completar múltiplos de 4
                for (int i = totalLinks; i < totalPages * 4; i++)
                {
                    fileLines.Add($"lnk{i + 1}=");
                }

                // Salva o conteúdo no arquivo
                File.WriteAllLines(pathLinksInc, fileLines);

                //MessageBox.Show("Links sincronizados com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao sincronizar links: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SyncSteamBanners()
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show($"You really want to replace banners with the founds by Steam IDs (Existents will be replaced)", "Confirm Auto Search IDs", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    string caminhoBanners = Path.Combine(caminhoArquivoAtual, "@Resources", "User", "Banners");
                    string caminhoTemp = Path.Combine(caminhoBanners, "@Temp");

                    if (picBanner.Image != null)
                    {
                        picBanner.Image.Dispose();
                        picBanner.Image = null;
                    }
                    if (File.Exists(caminhoImagem))
                    {
                        File.Delete(caminhoImagem);
                    }
                    if (Directory.Exists(caminhoTemp))
                    {
                        Directory.Delete(caminhoTemp, true);
                    }
                    if (!Directory.Exists(caminhoTemp))
                    {
                        Directory.CreateDirectory(caminhoTemp);
                    }

                    // Itera sobre os itens da lista
                    for (int i = 0; i < listGames.Items.Count; i++)
                    {
                        ListViewItem item = listGames.Items[i];
                        nomeJogo = item.Text; // Nome do jogo
                        string url = item.Tag as string; // URL armazenado no Tag

                        if (string.IsNullOrEmpty(nomeJogo) || string.IsNullOrEmpty(url))
                        {
                            continue; // Pula itens inválidos
                        }

                        // Verifica se o URL contém o padrão 'steam://rungameid/ID' e extrai o ID
                        string jogoIdExtraido = ExtractSteamGameId(url);

                        if (!string.IsNullOrEmpty(jogoIdExtraido))
                        {
                            // Se o ID foi encontrado, busca na lista de jogosSteam
                            if (jogosSteam.ContainsKey(jogoIdExtraido))
                            {
                                jogoId = jogosSteam[jogoIdExtraido];
                            }
                            else
                            {
                                // Caso não encontre o ID na lista, use o ID extraído
                                jogoId = jogoIdExtraido;
                            }

                            // Baixa e processa a imagem
                            caminhoImagem = BaixarImagemPorId();

                            if (!string.IsNullOrEmpty(caminhoImagem))
                            {
                                // Redimensiona e recorta a imagem
                                using (Image imagemOriginal = Image.FromFile(caminhoImagem))
                                {
                                    using (Image imagemRecortada = RedimensionarECortarImagem(imagemOriginal, 420, 170, Position.Middle))
                                    {
                                        // Salva a imagem recortada
                                        string caminhoImagemFinal = Path.Combine(caminhoBanners, $"banner{i + 1}.jpg");
                                        imagemRecortada.Save(caminhoImagemFinal, ImageFormat.Jpeg);
                                    }
                                }

                                // Após usar o arquivo, podemos excluí-lo
                                File.Delete(caminhoImagem);
                            }
                        }
                    }

                    // Exclui a pasta temporária após a sincronização
                    if (Directory.Exists(caminhoTemp))
                    {
                        Directory.Delete(caminhoTemp, true);
                    }

                    if (File.Exists(imagemSelecionada))
                    {
                        // Faz uma cópia temporária da imagem para evitar bloqueio no arquivo original
                        string caminhoTempPreview = Path.Combine(Path.GetTempPath(), "preview_temp.jpg");
                        File.Copy(imagemSelecionada, caminhoTempPreview, true);

                        // Carrega a imagem copiada no PictureBox
                        using (Image imagemPreview = Image.FromFile(caminhoTempPreview))
                        {
                            picBanner.Image = new Bitmap(imagemPreview); // Cria um novo Bitmap para liberar o arquivo
                        }

                        // Libera o arquivo temporário, se necessário, em outro ponto do código
                    }
                    else
                    {
                        picBanner.Image = null; // Limpa o PictureBox se a imagem não existir
                    }
                    MessageBox.Show("Sync realized with sucess!", "Sucess", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during syncronization: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ShowBannerForm(bool isSteam)
        {
            try
            {
                // Verifica se há um item selecionado
                if (itemSelecionado != null)
                {
                    string nomeJogo = itemSelecionado.Text; // Nome do jogo
                    string url = itemSelecionado.Tag as string; // URL armazenado no Tag
                    string jogoId = string.Empty;

                    // Verifica se o jogo está na lista jogosSteam
                    if (jogosSteam.ContainsKey(nomeJogo))
                    {
                        jogoId = jogosSteam[nomeJogo]; // Obtém o ID do jogo
                    }
                    else if (!string.IsNullOrEmpty(url))
                    {
                        // Tenta extrair o ID do URL caso não esteja na lista jogosSteam
                        jogoId = ExtractSteamGameId(url);
                    }

                    if (picBanner.Image != null)
                    {
                        picBanner.Image.Dispose();
                        picBanner.Image = null;
                    }

                    // Cria uma instância do BannerForm
                    bannerf = new BannerForm(caminhoArquivoAtual, jogoId, posicaoJogo, imagemSelecionada, isSteam);
                    bannerf.DarkMode = (MetroTheme == MetroThemeStyle.Dark);
                    bannerf.UpdateUserSettings(MetroTheme, MetroStyle);

                    // Exibe o BannerForm
                    bannerf.ShowDialog();
                    string posJg = Path.Combine(caminhoArquivoAtual, "@Resources", "User", "Banners", $"banner{posicaoJogo}.jpg");
                    if (File.Exists(posJg))
                    {
                        // Faz uma cópia temporária da imagem para evitar bloqueio no arquivo original
                        string caminhoTempPreview = Path.Combine(Path.GetTempPath(), "preview_temp.jpg");
                        File.Copy(posJg, caminhoTempPreview, true);

                        // Carrega a imagem copiada no PictureBox
                        using (Image imagemPreview = Image.FromFile(caminhoTempPreview))
                        {
                            picBanner.Image = new Bitmap(imagemPreview); // Cria um novo Bitmap para liberar o arquivo
                        }

                        // Libera o arquivo temporário, se necessário, em outro ponto do código
                    }
                    else
                    {
                        picBanner.Image = null; // Limpa o PictureBox se a imagem não existir
                    }

                }
                else
                {
                    MessageBox.Show("No file selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Erro ao abrir o BannerForm: caminhoArquivoAtual:{caminhoArquivoAtual} jogoId:{jogoId} posicaoJogo:{posicaoJogo} imagemSelecionada:{imagemSelecionada} {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BaixarImagemPorId()
        {
            try
            {
                string urlImagem = $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{jogoId}/library_hero.jpg";
                string caminhoBanners = Path.Combine(caminhoArquivoAtual, "@Resources", "User", "Banners");
                string caminhoTemp = Path.Combine(caminhoBanners, "@Temp");

                if (!Directory.Exists(caminhoTemp))
                {
                    Directory.CreateDirectory(caminhoTemp);
                }

                caminhoImagem = Path.Combine(caminhoTemp, $"{jogoId}_temp.jpg");

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(urlImagem, caminhoImagem); // Baixa a imagem
                }

                return caminhoImagem; // Retorna o caminho da imagem baixada
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error dowloading games image {jogoId}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private Image RedimensionarECortarImagem(Image imagemOriginal, int larguraFinal, int alturaFinal, Position posicao)
        {
            // Calcula a proporção necessária para atingir as dimensões mínimas
            float proporcaoLargura = (float)larguraFinal / imagemOriginal.Width;
            float proporcaoAltura = (float)alturaFinal / imagemOriginal.Height;
            float proporcao = Math.Max(proporcaoLargura, proporcaoAltura);

            // Calcula as novas dimensões mantendo a proporção original
            int novaLargura = (int)(imagemOriginal.Width * proporcao);
            int novaAltura = (int)(imagemOriginal.Height * proporcao);

            // Redimensiona a imagem para garantir que ambas as dimensões sejam maiores que o tamanho final
            Bitmap imagemRedimensionada = new Bitmap(novaLargura, novaAltura);
            imagemRedimensionada.SetResolution(imagemOriginal.HorizontalResolution, imagemOriginal.VerticalResolution);

            using (Graphics g = Graphics.FromImage(imagemRedimensionada))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                // Desenha a imagem redimensionada no novo bitmap
                g.DrawImage(imagemOriginal, 0, 0, novaLargura, novaAltura);
            }

            // Calcula as coordenadas para o corte baseado na posição
            int posX = 0, posY = 0;

            // Ajuste do corte conforme a posição selecionada
            switch (posicao)
            {
                case Position.TopLeft:
                    posX = 0;
                    posY = 0;
                    break;
                case Position.Top:
                    posX = (novaLargura - larguraFinal) / 2;
                    posY = 0;
                    break;
                case Position.TopRight:
                    posX = novaLargura - larguraFinal;
                    posY = 0;
                    break;
                case Position.Left:
                    posX = 0;
                    posY = (novaAltura - alturaFinal) / 2;
                    break;
                case Position.Middle:
                    posX = (novaLargura - larguraFinal) / 2;
                    posY = (novaAltura - alturaFinal) / 2;
                    break;
                case Position.Right:
                    posX = novaLargura - larguraFinal;
                    posY = (novaAltura - alturaFinal) / 2;
                    break;
                case Position.BottomLeft:
                    posX = 0;
                    posY = novaAltura - alturaFinal;
                    break;
                case Position.Bottom:
                    posX = (novaLargura - larguraFinal) / 2;
                    posY = novaAltura - alturaFinal;
                    break;
                case Position.BottomRight:
                    posX = novaLargura - larguraFinal;
                    posY = novaAltura - alturaFinal;
                    break;
            }

            // Cria a imagem final com as dimensões exatas
            Bitmap imagemFinal = new Bitmap(larguraFinal, alturaFinal);
            imagemFinal.SetResolution(imagemOriginal.HorizontalResolution, imagemOriginal.VerticalResolution);

            using (Graphics g = Graphics.FromImage(imagemFinal))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                // Desenha a área recortada da imagem redimensionada no bitmap final
                g.DrawImage(imagemRedimensionada, new Rectangle(0, 0, larguraFinal, alturaFinal),
                            new Rectangle(posX, posY, larguraFinal, alturaFinal), GraphicsUnit.Pixel);
            }

            imagemRedimensionada.Dispose(); // Libera a imagem redimensionada temporária da memória
            return imagemFinal;
        }

        // Função para extrair o ID do jogo da URL 'steam://rungameid/{ID}'
        private string ExtractSteamGameId(string url)
        {
            try
            {
                // Expressão regular para extrair o ID do jogo da URL
                var match = Regex.Match(url, @"steam://rungameid/(\d+)");
                if (match.Success)
                {
                    return match.Groups[1].Value; // Retorna o ID extraído
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error extracting ID from url: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
