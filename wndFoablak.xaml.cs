using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WPF_Kepnezegeto
{
	/// <summary>
	/// Interaction logic for wndFoablak.xaml
	/// </summary>
	public partial class wndFoablak : Window
	{
		/// <summary>
		/// A bélyegkép vezérlő alap szélessége.
		/// </summary>
		private double KépSzélesség;
		/// <summary>
		/// A bélyegkép vezérlő alap magassága.
		/// </summary>
		private double KépMagasság;
		/// <summary>
		/// Ennyi ideig tart az animáció.
		/// </summary>
		private TimeSpan tsAnimációIdő;
		/// <summary>
		/// Az animáció során ennyivel nő a kép szélessége.
		/// </summary>
		private double DSz;
		/// <summary>
		/// Az animáció során ennyivel nő a kép magassága.
		/// </summary>
		private double DM;

		/// <summary>
		/// A főablak konstruktora. Adattagok inicializálása és kezdőképek 
		/// /// betöltése.
		/// </summary>
		public wndFoablak()
		{
			InitializeComponent();
			//A kép vezérlő eredeti szélessége.
			KépSzélesség = 70;
			//A kép vezélő eredeti magassága. A felhasznált mintaképek mind 
			//1024x766-os méretűek.
			KépMagasság = KépSzélesség * 766 / 1024;
			//Az animáció során ennyivel nő a kép vezérlő szélessége.
			DSz = 30;
			//Az animáció során ennyivel nő a kép vezérlő magassága.
			DM = DSz * 766 / 1024;
			//Az animáció időigényének megadása.
			tsAnimációIdő = TimeSpan.FromMilliseconds(500);
		}

		/// <summary>
		/// Betölti a képeket a kiválasztott mappából, és kép vezérlők 
		/// formájában elhelyezi őket a WrapPanel-en.
		/// </summary>
		private void KépeketBetölt()
		{
			//A képeket tartalmazó könyvtár objektum létrehozása.
			DirectoryInfo dI = new DirectoryInfo(Title);
			//Töröljük a WrapPanel-en lévő vezérlők listáját.
			wpKépek.Children.Clear();
			try
			{
				//Lekérdezzük a .jpg kiterjesztésű állományokat a 
				//könyvtárból.
				FileInfo[] fI = dI.GetFiles("*.jpg");
				//Minden képet beolvasunk.
				foreach (FileInfo fajl in fI)
				{
					//A helyőrző létrehozása. Ez nagyobb kell legyen, //mint a kép vezérlő. Amikor növeljük a kép vezérlő //méretét, a helyőrzőt fogja kitölteni.
					Border bdHelyorzo = new Border();
					bdHelyorzo.Width = KépSzélesség + DSz;
					bdHelyorzo.Height = KépMagasság + DM;
					//Felvesszük a helyőrző a panelre.
					wpKépek.Children.Add(bdHelyorzo);
					//Létrehozunk egy kép objektumot, és betöltjük a //fájlból a képet.
					System.Windows.Controls.Image imKep = new()
					{
						Source = new BitmapImage(new Uri(fajl.FullName, UriKind.Absolute)),
						Width = KépSzélesség,
						Height = KépMagasság,
						//A kép a vezérlőn töltse ki a rendelkezésre álló //helyet az eredeti képarány megtartásával.
						Stretch = Stretch.Uniform,
						//A kép vezérlő a helyőrző közepére kerüljön.
						VerticalAlignment = System.Windows.VerticalAlignment.Center,
						HorizontalAlignment = System.Windows.HorizontalAlignment.Center
					};
					//Eseménykezelő rendelése az egérgomb lenyomásához.
					imKep.MouseDown += imKep_MouseDown;
					//Eseménykezelő rendelése az egér vezérlő fölé //érkezéséhez.
					imKep.MouseEnter += imKep_MouseEnter;
					//Eseménykezelő rendelése az egér vezérlő fölüli távozásához.
					imKep.MouseLeave += imKep_MouseLeave;
					//Kép elhelyezése a helyőrzőben.
					bdHelyorzo.Child = imKep;
				}
			}
			catch (Exception e)
			{
				System.Windows.MessageBox.Show(e.Message); //Hibaüzenet, ha nem sikerült valamelyik művelet.
			}
			if (wpKépek.Children.Count > 0)
			{
				//Beállítjuk a legelső képet nagynak.
				KepBeallit((System.Windows.Controls.Image)((Border)wpKépek.Children[0]).Child);
			}
		}

		/// <summary>
		/// A felhasználó az egeret elmozgatta a képről.
		/// </summary>
		/// <param name="sender">A kép vezérlő objektum.</param>
		/// <param name="e"></param>
		private void imKep_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			var imKep = (System.Windows.Controls.Image)sender;
			//A vízszintes méretváltoztatást leíró animáció objektum.
			DoubleAnimation dA = new DoubleAnimation();
			//Kezdőméret.
			dA.From = KépSzélesség + DSz;
			//Végső méret.
			dA.To = KépSzélesség;
			//Az animáció időtartama.
			dA.Duration = new Duration(tsAnimációIdő);
			//A függőleges méretváltoztatást leíró animáció objektum.
			DoubleAnimation dB = new DoubleAnimation();
			//Kezdőméret.
			dA.From = KépMagasság + DM;
			//Végső méret.
			dA.To = KépMagasság;
			//Az animáció időtartama.
			dB.Duration = new Duration(tsAnimációIdő);
			//A két animáció elindítása.
			imKep.BeginAnimation(WidthProperty, dA);
			imKep.BeginAnimation(HeightProperty, dB);
		}
		/// <summary>
		/// A felhasználó az egeret a kép vezérlő fele mozgatja.
		/// </summary>
		/// <param name="sender">A kép vezérlő objektum.</param>
		/// <param name="e"></param>
		private void imKep_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			//Az aktuális kép objektum.
			var imKep = (System.Windows.Controls.Image)sender;
			//A vízszintes méretváltoztatást leíró objektum.
			DoubleAnimation dA = new DoubleAnimation();
			//Kezdőméret.
			dA.From = KépSzélesség;
			//Végső méret.
			dA.To = KépSzélesség + DSz;
			//Az animáció időtartama.
			dA.Duration = new Duration(tsAnimációIdő);
			//A függőleges méretváltoztatást leíró objektum.
			DoubleAnimation dB = new DoubleAnimation();
			//Kezdőméret.
			dB.From = KépMagasság;
			//Végső méret.
			dB.To = KépMagasság + DM;
			//Az animáció időtartama.
			dB.Duration = new Duration(tsAnimációIdő);
			//A két animáció elindítása.
			imKep.BeginAnimation(WidthProperty, dA);
			imKep.BeginAnimation(HeightProperty, dB);
		}
		/// <summary>
		/// Beállítjuk a képet a nagy vezérlőben láthatónak.
		/// </summary>
		/// <param name="imKep"></param>
		private void KepBeallit(System.Windows.Controls.Image imKep)
		{
			//A kép forrása.
			imNagyKép.Source = imKep.Source;
			//A kép alatt megjeleneítjük az állomány nevét.
			tbKépNév.Text = imNagyKép.Source.ToString();
		}

		/// <summary>
		/// Egérgomb lenyomása egy kép vezérlőn.
		/// </summary>
		/// <param name="sender">A kép vezérlő.</param>
		/// <param name="e"></param>
		private void imKep_MouseDown(object sender, MouseButtonEventArgs e)
		{
			//Beállítjuk a képet a nagy vezérlőn láthatónak.
			KepBeallit((System.Windows.Controls.Image)sender);
		}

		/// <summary>
		/// A képeket tartalmazó könyvtár kiválasztása.
		/// </summary>
		/// <param name="sender">Az eseményt előidéző nyomógomb.</param>
		/// <param name="e">Kiegészítő paraméterek.</param>
		private void btKönyvtárVálasztó_Click(object sender, RoutedEventArgs e)
		{
			//Mappaválasztó párbeszédablak objektum létrehozása.
			var dlg = new System.Windows.Forms.FolderBrowserDialog();
			//Kezdőkönyvtár beállítása az exe könyvtárára.
			dlg.RootFolder = Environment.SpecialFolder.Desktop;

			dlg.SelectedPath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName;
			//Párbeszédablak megjeleneítése.
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				//A mappa elérési útvonalának átmásolása az ablak fejlécbe.
				Title = dlg.SelectedPath;
				//A képek betöltése a kiválasztott mappából, és elhelyezésük kép vezérlők formájában a WrapPanel-en.
				KépeketBetölt();
			}
		}
	}
}