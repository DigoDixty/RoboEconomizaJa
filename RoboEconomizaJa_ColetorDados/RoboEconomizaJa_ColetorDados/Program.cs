using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboEconomizaJa_ColetorDados
{

    class Program
    {
        
        
        
        static void Main(string[] args)
        {

            //ListarMarcas();
            //ListarProdutos();
            //LimpaArquivo();
            ListarDetalhesProdutos();
            //ListarSubCategorias();
        }

        static void ListarDetalhesProdutos()
        {
            string dir = Properties.Settings.Default.dir_default.ToString(); // ou dir_temp
            //string dir = Properties.Settings.Default.dir_temp;

            string FileDetalhesProdutos_tmp = "C:\\" + dir + "\\file_detalhes_produtos_tmp.txt";
            string FileDetalhesProdutos_end = "C:\\" + dir + "\\file_detalhes_produtos_fim.txt";
            string FileDetalhesProdutos_end = "C:\\" + dir + "\\log_file.txt";

            //string FileProdutos_end = "C:\\" + dir + "\\Todos_Produtos.txt";
            string FileProdutos_end = "C:\\" + dir + "\\file_produtos_test.txt";


            string pagesource = "";
            string LineLoad_Web;
            string st_linha ;

            StreamReader FileProdutos = new StreamReader(FileProdutos_end);
            while ((LineLoad_Web = FileProdutos.ReadLine()) != null)
            {

                string departamento = "";
                string url = "";

                departamento = LineLoad_Web.Substring(0, LineLoad_Web.IndexOf("|"));
                url = LineLoad_Web.Substring(LineLoad_Web.IndexOf("|") + 1);

                IWebDriver driver = new ChromeDriver();
                //driver.Manage().Timeouts(600);
                driver.Navigate().GoToUrl(url);
                pagesource = driver.PageSource;

                string st_busca_tit = "'productName': '";
                string titulo = pagesource.Substring(pagesource.IndexOf(st_busca_tit) + 16);
                titulo = titulo.Substring(0, titulo.IndexOf("'"));

                //string st_busca_dep = "'departmentName'::";
                //string Departamento = pagesource.Substring(pagesource.IndexOf(st_busca_dep) + 18);
                //Departamento = Departamento.Substring(0, Departamento.IndexOf("'"));

                string st_busca_category = "'category':";
                string st_category = pagesource.Substring(pagesource.IndexOf(st_busca_category) + 13);
                st_category = st_category.Substring(0, st_category.IndexOf("'") );

                string st_busca_img = "'productImage':";
                string EndImagemWeb = pagesource.Substring(pagesource.IndexOf(st_busca_img) + 17);
                EndImagemWeb = EndImagemWeb.Substring(0, EndImagemWeb.IndexOf("'"));
                
                string detalhes = (pagesource.Substring(pagesource.IndexOf("<div class=" + "\"" + "product-descriptions" + "\">") ) );
                detalhes = (detalhes.Substring(0,detalhes.IndexOf("<script>")));

                string marca2 = new string(titulo.Reverse().ToArray());
                marca2 = marca2.Substring(0, marca2.IndexOf("-"));
                string Marca = new string(marca2.Reverse().ToArray()).Trim();

                string st_busca_short_desc = "<div class=" + "\"" + "product-shortDescription" + "\">";
                string short_desc = pagesource.Substring(pagesource.IndexOf(st_busca_short_desc));
                short_desc = short_desc.Substring(short_desc.IndexOf(">") + 1);
                short_desc = short_desc.Trim();
                short_desc = short_desc.Substring(0, short_desc.IndexOf("</div>"));

                if (short_desc.IndexOf("<br>") > -1)
                {
                    short_desc = short_desc.Substring(0,short_desc.IndexOf("<br>"));
                }
                short_desc = short_desc.Replace("<div>", "");
                short_desc = short_desc.Replace("</div>", "");

                string st_busca_price = "var originalPrice = " + "\"";
                string short_price = pagesource.Substring(pagesource.IndexOf(st_busca_price) + 21);
                short_price = "Preço: " + short_price.Substring(0, short_price.IndexOf(";") - 1 );

                string variacoes = "";
                string st_busca_variacoes = "<div class=" + "\"" + "product-variants-scroll" + "\">";
                if (pagesource.IndexOf(st_busca_variacoes) > -1)
                {
                    variacoes = pagesource.Substring(pagesource.IndexOf(st_busca_variacoes));
                    variacoes = variacoes.Substring(0, variacoes.IndexOf("<script>"));
                }
                else
                {
                    variacoes = "UNICO";
                }

                //string linha_escrita = titulo + "|" + short_desc+ "|" + short_price + "|" + detalhes+ "|" + variacoes;

                using (StreamWriter FileWrite_tmp = new StreamWriter(FileDetalhesProdutos_tmp))
                {
                    FileWrite_tmp.WriteLine("sub_categoria_ini: " + st_category + " :sub_categoria_fim");
                    FileWrite_tmp.WriteLine("titulo_ini: " + titulo + " :titulo_fim");
                    FileWrite_tmp.WriteLine("short_desc_ini: " + short_desc + " :short_desc_fim");
                    FileWrite_tmp.WriteLine("short_price_ini: " + short_price + " :short_price_fim");
                    FileWrite_tmp.WriteLine("detalhes_ini: " + detalhes + " :detalhes_fim");
                    FileWrite_tmp.WriteLine("variacoes_ini: " + variacoes + " :variacoes_fim");

                    FileWrite_tmp.Flush();
                    FileWrite_tmp.Close();
                }

                StreamReader FileLoad2 = new StreamReader(FileDetalhesProdutos_tmp);

                string NomeProduto = "";
                string SubCategoria = "";
                string ShortDescricao = "";
                string Preco = "";
                
                string cod_ind = "nononononononoonononononono";
                string cod_carc = "nononononononoonononononono";
                string cod_item_inc = "nononononononoonononononono";
                string cod_video = "nononononononoonononononono";

                string pega_prox_linha_ind = ""; // pegar a proxima linha até a div.

                string indicacoes = "";
                string caracteristicas = "";
                string itensinclusos = "";
                string video = "";
                string variacao = "";
                string variacao_ult = "";

                string pega_prox_linha_variacao = "";
                string grava_linha = "";

                string nome_prod_variacao = "";
                string preco_prod_variacao = "";
                string img_prod_variacao = "";

                string pega_prox_linha_nome_produto = "";


                while ((st_linha = FileLoad2.ReadLine()) != null)
                {

                    if (st_linha.IndexOf("sub_categoria_ini: ") > -1)
                    {
                        SubCategoria = st_linha.Substring(st_linha.IndexOf("sub_categoria_ini: ") + 18).Trim();
                        SubCategoria = SubCategoria.Substring(0, SubCategoria.IndexOf(":sub_categoria_fim")).Trim();
                    }


                    if (st_linha.IndexOf("titulo_ini: ") > -1)
                    {
                        NomeProduto = st_linha.Substring(st_linha.IndexOf("titulo_ini: ") + 12).Trim();
                        NomeProduto = NomeProduto.Substring(0, NomeProduto.IndexOf(":titulo_fim")).Trim();
                        NomeProduto = NomeProduto.Replace("Escolha a cor","");
                        NomeProduto = NomeProduto.Replace("escolha a cor", "");
                        NomeProduto = NomeProduto.Replace(" .", ".").Trim();
                        NomeProduto = NomeProduto.Replace("..", ".").Trim();

                    }

                    if (st_linha.IndexOf("short_desc_ini: ") > -1)
                    {
                        ShortDescricao = st_linha.Substring(st_linha.IndexOf("short_desc_ini: ") + 15).Trim();
                        if (ShortDescricao.IndexOf(":short_desc_fim") > -1)
                        {
                            ShortDescricao = ShortDescricao.Substring(0, ShortDescricao.IndexOf(":short_desc_fim"));
                        }
                        ShortDescricao = ShortDescricao.Replace("Escolha a cor", "cor").Trim();
                        ShortDescricao = ShortDescricao.Replace("escolha a cor", "cor").Trim();
                        ShortDescricao = ShortDescricao.Replace("Escolha tamanho e força", "").Trim();
                        ShortDescricao = ShortDescricao.Replace("escolha tamanho e força", "").Trim();
                        ShortDescricao = ShortDescricao.Replace("</span>", "").Trim();
                        ShortDescricao = ShortDescricao.Replace("<span>", "").Trim();
                        ShortDescricao = ShortDescricao.Replace(" .", ".").Trim();
                        ShortDescricao = ShortDescricao.Replace("..", ".").Trim();

                        NomeProduto = NomeProduto + " - " + ShortDescricao;

                    }


                    if (st_linha.IndexOf("short_price_ini: Preço: ") > -1)
                    {
                        Preco = st_linha.Substring(st_linha.IndexOf("short_price_ini: Preço: ") + 24).Trim();
                        Preco = Preco.Substring(0, Preco.IndexOf(":short_price_fim")).Trim();
                        Preco = Preco.Replace("R$","").Trim();

                    }

                    if (st_linha.ToLower().IndexOf("<span>indi") > -1 && (st_linha.ToLower().IndexOf("o</span>") > -1 || st_linha.ToLower().IndexOf("es</span>") > -1) )
                    {

                        cod_ind = st_linha.Substring(st_linha.IndexOf("data-tab=") + 10).Trim();
                        cod_ind = cod_ind.Substring(0, cod_ind.IndexOf("\""));
                    }

                    if (st_linha.ToLower().IndexOf("<span>ite") > -1 && st_linha.ToLower().IndexOf("</span>") > -1)
                    {
                        cod_item_inc = st_linha.Substring(st_linha.IndexOf("data-tab=") + 10).Trim();
                        cod_item_inc = cod_item_inc.Substring(0, cod_item_inc.IndexOf("\""));
                    }

                    if (st_linha.ToLower().IndexOf("<span>caracter") > -1 && st_linha.ToLower().IndexOf("</span>") > -1)
                    {
                        cod_carc = st_linha.Substring(st_linha.IndexOf("data-tab=") + 10).Trim();
                        cod_carc = cod_carc.Substring(0, cod_carc.IndexOf("\""));
                    }

                    if (st_linha.ToLower().IndexOf("<span>v") > -1 && st_linha.ToLower().IndexOf("deo</span>") > -1)
                    {
                        cod_video = st_linha.Substring(st_linha.IndexOf("data-tab=") + 10).Trim();
                        cod_video = cod_video.Substring(0, cod_video.IndexOf("\""));
                    }

                    if (pega_prox_linha_ind == "sim_ind")
                    {
                        string indicacoes_2 = st_linha.Replace("<p>", "").Replace("</p>", "");
                        indicacoes_2 = indicacoes_2.Trim().Replace("</div>", "").Replace("&nbsp", "").Trim();
                        indicacoes_2 = indicacoes_2.Replace("<strong>", "").Replace("</strong>", "").Trim();
                        indicacoes = indicacoes + "<br>" + indicacoes_2;
                        if (st_linha.IndexOf("</div>") > -1)
                        {
                            pega_prox_linha_ind = "";
                        }
                    }

                    if (pega_prox_linha_ind == "sim_car")
                    {
                        string caracteristicas_2 = st_linha.Replace("<p>", "").Replace("</p>", "");
                        caracteristicas_2 = caracteristicas_2.Trim().Replace("</div>", "").Replace("&nbsp", "");
                        caracteristicas_2 = caracteristicas_2.Replace("<strong>", "").Replace("</strong>", "").Trim();
                        caracteristicas = caracteristicas + "<br>" + caracteristicas_2;
                        if (st_linha.IndexOf("</div>") > -1)
                        {
                            pega_prox_linha_ind = "";
                        }
                    }

                    if (pega_prox_linha_ind == "sim_item")
                    {
                        string itensinclusos_2 = st_linha.Replace("<p>", "").Replace("</p>", "");
                        itensinclusos_2 = itensinclusos_2.Trim().Replace("</div>", "").Replace("&nbsp", "");
                        itensinclusos = itensinclusos + "<br>" + itensinclusos_2;
                        if (st_linha.IndexOf("</div>") > -1)
                        {
                            pega_prox_linha_ind = "";
                        }
                    }

                    if (pega_prox_linha_ind == "sim_video")
                    {
                        string video_2 = st_linha.Substring(st_linha.IndexOf("www.youtube.com/"));
                        video_2 = "http://" + video_2.Substring(0, video_2.IndexOf("\""));
                        video = video_2;
                        pega_prox_linha_ind = "";
                    }

                    if (st_linha.IndexOf(cod_ind) > -1 && st_linha.IndexOf("product-descriptions-content") > -1)
                    {
                        pega_prox_linha_ind = "sim_ind";
                    }
                    if (st_linha.IndexOf(cod_carc) > -1 && st_linha.IndexOf("product-descriptions-content") > -1)
                    {
                        pega_prox_linha_ind = "sim_car";
                    }
                    if (st_linha.IndexOf(cod_item_inc) > -1 && st_linha.IndexOf("product-descriptions-content") > -1)
                    {
                        pega_prox_linha_ind = "sim_item";
                    }
                    if (st_linha.IndexOf(cod_video) > -1 && st_linha.IndexOf("product-descriptions-content") > -1)
                    {
                        pega_prox_linha_ind = "sim_video";
                    }

                    if (st_linha.IndexOf("variacoes_ini:") > -1)
                    {
                        pega_prox_linha_variacao = "sim_conteudo_variacao";
                    }

                    if (pega_prox_linha_variacao == "sim_conteudo_variacao")
                    {

                        if (pega_prox_linha_nome_produto == "pegar")
                        {
                            int contaEspaco = 0;

                            nome_prod_variacao = st_linha.Substring(st_linha.IndexOf("<h2>") + 4);


                             nome_prod_variacao = nome_prod_variacao.Substring(0, nome_prod_variacao.IndexOf("</h2>")).Trim();
                            if (st_linha.IndexOf("(") > -1)
                            {
                                nome_prod_variacao = nome_prod_variacao.Substring(0, nome_prod_variacao.IndexOf("(")).Trim();
                            }
                            else
                            {
                                contaEspaco = nome_prod_variacao.Length - nome_prod_variacao.Replace(" ", "").Length;
                                if (contaEspaco == 1)
                                {
                                    nome_prod_variacao = nome_prod_variacao.Substring(0, nome_prod_variacao.IndexOf(" "));
                                }

                                if (contaEspaco >= 2)
                                {
                                    string pega_meio_nome = nome_prod_variacao.Substring(nome_prod_variacao.IndexOf(" ") + 1);
                                    pega_meio_nome = pega_meio_nome.Substring(0, pega_meio_nome.IndexOf(" "));
                                    nome_prod_variacao = nome_prod_variacao.Substring(0, nome_prod_variacao.IndexOf(" ")) + " " + pega_meio_nome;
                                }
                            }

                            pega_prox_linha_nome_produto = "";
                        }

                        if (st_linha.IndexOf("product-variant-title") > -1)
                        {
                            if (st_linha.IndexOf("</h2>") > -1)
                            {
                                nome_prod_variacao = st_linha.Substring(st_linha.IndexOf("product-variant-title") + 23);
                                nome_prod_variacao = nome_prod_variacao.Substring(0, nome_prod_variacao.IndexOf(" "));
                            }
                            else
                            {
                                pega_prox_linha_nome_produto = "pegar";
                            }
                        }

                        if (st_linha.IndexOf("product-variant-price") > -1)
                        {
                            preco_prod_variacao = st_linha.Substring(st_linha.IndexOf("data-price=") + 12);
                            preco_prod_variacao = preco_prod_variacao.Substring(0, preco_prod_variacao.IndexOf("\""));
                        }

                        if (st_linha.IndexOf("product-variant-image") > -1)
                        {
                            img_prod_variacao = st_linha.Substring(st_linha.IndexOf("data-variantimage=") + 19);
                            img_prod_variacao = img_prod_variacao.Substring(0, img_prod_variacao.IndexOf("\""));
                            if (img_prod_variacao != "")
                            {
                                img_prod_variacao = img_prod_variacao.Substring(0, img_prod_variacao.IndexOf("?")).Trim();
                                EndImagemWeb = img_prod_variacao;
                            }
                        }

                        if (st_linha.IndexOf("</li>") > -1 || st_linha.IndexOf("</tr>") > -1)
                        {
                            variacao = NomeProduto + " - " + nome_prod_variacao.ToLower() + "¿" + preco_prod_variacao + "¿" + img_prod_variacao;
                            variacao = variacao.Replace(". cor. - ", " - cor ");
                            variacao = variacao.Replace(". cor. - ", " - cor ");
                            variacao = variacao.Replace(". - ", " - ");
                            if (variacao != variacao_ult)
                            {
                                grava_linha = "gravar";
                            }
                        }
                    }

                    if (st_linha.IndexOf("UNICO") > -1)
                    {
                        variacao = "UNICO¿UNICO¿UNICO";
                        grava_linha = "gravar";
                    }

                    //Gravacao de registro precisa ser feito de acordo com as variacoes do produto:

                    if (grava_linha == "gravar") // e fazer download da imagem
                    {
                        if (img_prod_variacao == "" && EndImagemWeb != "")
                        {
                            if (EndImagemWeb.IndexOf("?") > -1)
                            {
                                EndImagemWeb = EndImagemWeb.Substring(0, EndImagemWeb.IndexOf("?")).Trim();
                            }
                        }

                        string ImagemSiteDownloadedGd = "";
                        string ImagemSiteDownloadedPq = "";

                        if (EndImagemWeb != "")
                        {
                            if (nome_prod_variacao == "")
                            {
                                ImagemSiteDownloadedGd = NomeProduto.Replace(".","");
                                ImagemSiteDownloadedGd = ImagemSiteDownloadedGd.ToLower();
                            }
                            else
                            {
                                ImagemSiteDownloadedGd = NomeProduto + "_" + nome_prod_variacao.Replace(".", "");
                                ImagemSiteDownloadedGd = ImagemSiteDownloadedGd.Replace(". cor.", "");
                                ImagemSiteDownloadedGd = ImagemSiteDownloadedGd.Replace("\"", "");
                                ImagemSiteDownloadedGd = ImagemSiteDownloadedGd.Replace("=", "");
                                ImagemSiteDownloadedGd = ImagemSiteDownloadedGd.ToLower();
                                ImagemSiteDownloadedGd = ImagemSiteDownloadedGd.Replace("._.", ".");
                            }
                            
                            ImagemSiteDownloadedGd = ImagemSiteDownloadedGd.Replace("-", "").Replace(" .", ".").Replace(" ", "_").Replace("__","_");
                            ImagemSiteDownloadedGd = ImagemSiteDownloadedGd.Replace("/", "").Replace(",","");
                            ImagemSiteDownloadedGd = ImagemSiteDownloadedGd.Replace(":", "").Replace("%", "").Replace("+", "");
                            ImagemSiteDownloadedPq = ImagemSiteDownloadedGd + "_pq.jpg";
                            ImagemSiteDownloadedGd = ImagemSiteDownloadedGd + "_gd.jpg";
                            // Pequeno e grande.

                            string dir_subcategoria = "";
                            if (SubCategoria != "")
                            {
                                dir_subcategoria = "C:\\" + dir + "\\test_img\\" + departamento + "\\" + SubCategoria;
                            }
                            else
                            {
                                dir_subcategoria = "C:\\" + dir + "\\test_img\\" + departamento;
                            }

                            if (!Directory.Exists(dir_subcategoria))

                            {
                                Directory.CreateDirectory(dir_subcategoria);
                            }

                            string localPathPq = dir_subcategoria + "\\" + ImagemSiteDownloadedPq;
                            string localPathGd = dir_subcategoria + "\\" + ImagemSiteDownloadedGd;

                            var client = new WebClient();

                            client.DownloadFile(EndImagemWeb, localPathGd);
                            client.DownloadFile(EndImagemWeb, localPathPq);
                        }

                        // regras para isolar para analise
                        // sem imagens 
                        // sem subcategoria

                        string indicador = "inserir";

                        if (SubCategoria == "")
                        {
                            indicador = "analisar_sem_subcategoria";
                        }
                        if (EndImagemWeb == "")
                        {
                            indicador = "analisar_sem_imagem";
                        }


                        string linha = "";
                        linha = indicador;
                        linha = linha + "¿" + variacao;

                        linha = linha + "¿" + departamento;
                        linha = linha + "¿" + SubCategoria;
                        linha = linha + "¿" + Marca;
                        linha = linha + "¿" + NomeProduto.Replace("..",".");
                        linha = linha + "¿" + Preco;
                        linha = linha + "¿" + ImagemSiteDownloadedPq;
                        linha = linha + "¿" + ImagemSiteDownloadedGd;
                        linha = linha + "¿" + EndImagemWeb;

                        linha = linha + "¿";
                        if (indicacoes != "")
                        {
                            linha = linha + "<b>Especificação:</b>" + indicacoes.Substring(4);
                        }
                        if (caracteristicas != "")
                        {
                            linha = linha + "<br><b>Caracteristicas:</b>" + caracteristicas.Substring(4);
                        }
                        if (itensinclusos != "")
                        {
                            linha = linha + "<br><b>Itens Inclusos:</b>" + itensinclusos.Substring(4);
                        }
                        if (video != "")
                        {
                            linha = linha + "<br><b>Videos</b>" + video;
                        }

                        linha = linha + "¿" + url;

                        linha = linha.Replace("<div>", "").Replace("</div>", "");  // limpeza
                        linha = linha.Replace("<span>", "").Replace("</span>",""); // limpeza
                        linha = linha.Replace("<br><br>", "<br>"); // limpeza
                        linha = linha.Replace("<br><br>", "<br>"); // limpeza
                        linha = linha.Replace("'", ""); //retirar aspa simples.

                        grava_arquivo(FileDetalhesProdutos_end, linha);
                        grava_linha = "";
                        variacao_ult = variacao;
                        // pendencias:
                        // obter sub-catedoria.
                        // criar diretorio (sub-categoria).
                        // limpar dados.
                    }
                }

                FileLoad2.Close();
                driver.Quit();
            }
            FileProdutos.Close();
        }

        static void LimpaArquivo()
        {
            string dir = Properties.Settings.Default.dir_default.ToString(); // ou dir_temp
            //string dir = Properties.Settings.Default.dir_temp;


            string FileProdutos_end = "C:\\" + dir + "\\file_produtos_fim.txt";
            string FileProdutos_new = "C:\\" + dir + "\\file_produtos_end.txt";
            string LineLoad;

            StreamReader FileLoad2 = new StreamReader(FileProdutos_end);
            while ((LineLoad = FileLoad2.ReadLine()) != null)
            {

                string[] RegexArray = Regex.Split(LineLoad, "/");

                using (StreamWriter FileMarca = File.AppendText(FileProdutos_new))
                {
                    FileMarca.WriteLine(RegexArray[5] + "|" + LineLoad);
                }
            }
            FileLoad2.Close();

        }

        static void ListarProdutos()
        {
            string dir = Properties.Settings.Default.dir_default.ToString(); // ou dir_temp
            //string dir = Properties.Settings.Default.dir_temp;


            string FileProdutos_tmp = "C:\\" + dir + "\\file_produtos_tmp.txt";
            string FileProdutos_end = "C:\\" + dir + "\\file_produtos_fim.txt";
            string pagesource = "";
            string LineLoad;

            List<String> L = ListaDepartamentos();
            foreach (string web in L)

            {
                int count_pag = 1;
                count_pag = int.Parse(web.Substring(web.IndexOf("|") + 1));
                while (count_pag >= 1)
                {
                    string web_cut = web.Substring(0, web.IndexOf("/pag/")) + "/pag/" + count_pag + "/ordem/name";
                    IWebDriver driver = new ChromeDriver();
                    driver.Navigate().GoToUrl(web_cut);
                    pagesource = driver.PageSource;

                    using (StreamWriter FileWrite_tmp = new StreamWriter(FileProdutos_tmp))
                    {
                        FileWrite_tmp.WriteLine(pagesource);
                        FileWrite_tmp.Flush();
                        FileWrite_tmp.Close();
                    }
                    
                    int i = 0;
                    StreamReader FileLoad2 = new StreamReader(FileProdutos_tmp);
                    while ((LineLoad = FileLoad2.ReadLine()) != null)
                    {
                        string line_test01 = "";
                        if (LineLoad.IndexOf("href=\"https://") > -1)
                        {
                            line_test01 = LineLoad.Substring(LineLoad.IndexOf("href=") + 6);
                            if (line_test01.IndexOf("produto") > -1 && (line_test01.IndexOf("promocao") < 0 && line_test01.IndexOf("departamento") < 0 ) )
                            {
                                
                                using (StreamWriter FileMarca = File.AppendText(FileProdutos_end))
                                {
                                 FileMarca.WriteLine(web_cut + "|" +line_test01.Substring(0, line_test01.IndexOf(">") - 1));
                                }
                            }
                        }
                    }

                    FileLoad2.Close();
                    File.Delete(FileProdutos_tmp);

                    string pagesource_cut = pagesource.Substring(pagesource.IndexOf("href="));
                    count_pag = count_pag - 1;
                    driver.Quit();
                    Console.WriteLine("URL: " + web);
                }
                
            }

        }

        static void grava_arquivo(string var01, string var02)
        {
            using (StreamWriter FileMarca = File.AppendText(var01))
            {
                FileMarca.WriteLine(var02);
                Console.WriteLine(var02);
            }
        }

        static void ListarSubCategorias()
        {
            string dir = Properties.Settings.Default.dir_default.ToString(); // ou dir_temp
            //string dir = Properties.Settings.Default.dir_temp;


            string FileMarcaTmp = "C:\\" + dir + "\\ListarSubCategorias.tmp";
            string FileListaMarcas = "C:\\" + dir + "\\ListarSubCategorias.txt";

            string pagesource = "";
            string LineLoad;
            int CountLine = 0;
            int FindWord = 0;

            List<String> L = ListaDepartamentos();
            foreach (string web in L)

            {
                string web_cut = web.Substring(0, web.IndexOf("|"));
                IWebDriver driver = new ChromeDriver();
                driver.Navigate().GoToUrl(web_cut);
                pagesource = driver.PageSource;

                string departament = web.Substring(52);

                string pagesource_cut = pagesource.Substring(pagesource.IndexOf("Marca</h2>"));
                pagesource_cut = pagesource_cut.Substring(0, pagesource_cut.IndexOf("</nav>"));

                string qtd_produtos = pagesource.Substring(pagesource.IndexOf("Produtos encontrados: <strong>") + 30);
                qtd_produtos = qtd_produtos.Substring(0, qtd_produtos.IndexOf("</strong>"));

                driver.Quit();
                using (StreamWriter FileMarca = File.AppendText(FileMarcaTmp))
                {
                    FileMarca.WriteLine(departament + "|" + qtd_produtos + "|" + pagesource_cut);
                }
                Console.WriteLine("URL: " + web);
            }

            System.IO.StreamReader FileLoad = new System.IO.StreamReader(FileMarcaTmp);

            while ((LineLoad = FileLoad.ReadLine()) != null)
            {
                CountLine++;
                FindWord = LineLoad.IndexOf("<small>");
                if (FindWord > 0)
                {
                    using (StreamWriter FileMarca = File.AppendText(FileListaMarcas))
                    {
                        string st_01 = LineLoad.Substring(LineLoad.IndexOf("\">") + 2);
                        st_01 = st_01.Substring(0, st_01.IndexOf("<") - 1);

                        string st_02 = LineLoad.Substring(LineLoad.IndexOf(".com.br") + 8);
                        st_02 = st_02.Substring(0, st_02.IndexOf("/") - 1);

                        FileMarca.WriteLine(st_01 + "|" + st_02);
                    }
                    FindWord = 0;
                }
            }
            FileLoad.Close();
        }

        static void ListarMarcas()
        {
            string dir = Properties.Settings.Default.dir_default.ToString(); // ou dir_temp
            //string dir = Properties.Settings.Default.dir_temp;


            string FileMarcaTmp = "C:\\" + dir + "\\ListaMarcas.tmp";
            string FileListaMarcas = "C:\\" + dir + "\\ListaMarcas.txt";

            string pagesource = "";
            string LineLoad;
            int CountLine = 0;
            int FindWord = 0;
            
            List<String> L = ListaDepartamentos();
            foreach (string web in L)

            {
                string web_cut = web.Substring(0,web.IndexOf("|"));
                IWebDriver driver = new ChromeDriver();
                driver.Navigate().GoToUrl(web_cut);
                pagesource = driver.PageSource;

                string departament = web.Substring(52);

                string pagesource_cut = pagesource.Substring(pagesource.IndexOf("Marca</h2>"));
                pagesource_cut = pagesource_cut.Substring(0, pagesource_cut.IndexOf("</nav>"));

                string qtd_produtos = pagesource.Substring(pagesource.IndexOf("Produtos encontrados: <strong>") + 30);
                qtd_produtos = qtd_produtos.Substring(0, qtd_produtos.IndexOf("</strong>"));

                driver.Quit();
                using (StreamWriter FileMarca = File.AppendText(FileMarcaTmp))
                {
                    FileMarca.WriteLine(departament + "|"+ qtd_produtos + "|" + pagesource_cut);
                }
                Console.WriteLine("URL: " + web);
            }

            System.IO.StreamReader FileLoad = new System.IO.StreamReader(FileMarcaTmp);
            while ((LineLoad = FileLoad.ReadLine()) != null)
            {
                CountLine++;
                FindWord = LineLoad.IndexOf("<small>");
                if (FindWord > 0)
                {
                    using (StreamWriter FileMarca = File.AppendText(FileListaMarcas))
                    {
                        string st_01 = LineLoad.Substring(LineLoad.IndexOf("\">") + 2);
                        st_01 = st_01.Substring(0, st_01.IndexOf("<") - 1);

                        string st_02 = LineLoad.Substring(LineLoad.IndexOf(".com.br") + 8);
                        st_02 = st_02.Substring(0, st_02.IndexOf("/") - 1);

                        FileMarca.WriteLine(st_01 + "|" + st_02);
                    }
                    FindWord = 0;
                }
            }
            FileLoad.Close();
        }

        public static List<string> ListaDepartamentos()
        {

            
            List<string> ListaDepartamento = new List<string>();

            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854596/anestesicos-e-agulha-gengival/pag/2/ordem/name|2");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854599/biosseguranca/pag/14/ordem/name|14");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854613/brocas/pag/24/ordem/name|24");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/855240/cadeiras-odontologicas/pag/1/ordem/name|1");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854614/cimentos/pag/7/ordem/name|7");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854635/cirurgia-e-periodontia/pag/6/ordem/name|6");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854653/dentistica-e-estetica/pag/43/ordem/name|43");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854690/descartaveis/pag/9/ordem/name|9");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854715/endodontia/pag/18/ordem/name|18");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854829/fotografia/pag/2/ordem/name|2");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/855587/harmonizacao-orofacial/pag/2/ordem/name|2");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/855166/higiene-oral/pag/11/ordem/name|11");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854843/implantodontia/pag/7/ordem/name|7");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854853/instrumentais/pag/56/ordem/name|56");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854918/moldagem/pag/13/ordem/name|13");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/855419/moveis-para-laboratorio/pag/1/ordem/name|1");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854895/ortodontia/pag/61/ordem/name|61");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854716/perifericos-e-pecas-de-mao/pag/32/ordem/name|32");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854942/prevencao-e-profilaxia/pag/6/ordem/name|6");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854756/protese/pag/81/ordem/name|81");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/854734/radiologia/pag/4/ordem/name|4");
            ListaDepartamento.Add("https://www.dentalcremer.com.br/departamento/855134/vestuario/pag/12/ordem/name|12");



            return ListaDepartamento;
        }
    }
}
