using System.Text.Json;

namespace aPls
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            string nombrePokemon = txtNombre.Text.Trim();

            if (string.IsNullOrEmpty(nombrePokemon))
            {
                MessageBox.Show("Por favor, ingresa un nombre de Pokémon.");
                return;
            }

            try
            {
                var datos = await ObtenerPokemonAsync(nombrePokemon);

                if (datos != null)
                {
                    lblNombre.Text = $"Nombre: {datos.name}";
                    lblPeso.Text = $"Peso: {datos.weight}";
                    lblAltura.Text = $"Altura: {datos.height}";

                    // Mostrar habilidades en el ListBox
                    lstHabilidades.Items.Clear();
                    foreach (var habilidad in datos.abilities)
                    {
                        lstHabilidades.Items.Add(habilidad.ability.name);
                    }

                    // Cargar imagen en PictureBox
                    using (HttpClient client = new HttpClient())
                    {
                        var imageBytes = await client.GetByteArrayAsync(datos.sprites.front_default);
                        using (var ms = new MemoryStream(imageBytes))
                        {
                            pictureBox1.Image = Image.FromStream(ms);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se encontró el Pokémon. Verifica el nombre.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}");
            }
        }

        private async Task<Pokemon> ObtenerPokemonAsync(string nombre)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"https://pokeapi.co/api/v2/pokemon/{nombre.ToLower()}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Pokemon>(json);
                }
                else
                {
                    return null;
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombrePokemon = txtNombre.Text.Trim();

            if (string.IsNullOrEmpty(nombrePokemon))
            {
                MessageBox.Show("Por favor, busca un Pokémon antes de guardar.");
                return;
            }

            try
            {
                var datos = ObtenerPokemonAsync(nombrePokemon).Result; // Obtener los datos del Pokémon
                GuardarResultadosEnTxt(datos); // Guardar los datos en un archivo
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al guardar los resultados: {ex.Message}");
            }
        }

        private void GuardarResultadosEnTxt(Pokemon datos)
        {
            if (datos == null)
            {
                MessageBox.Show("No hay datos para guardar.");
                return;
            }

            // Ruta del archivo en el escritorio del usuario
            string rutaArchivo = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"{datos.name}.txt");

            try
            {
                // Crear el contenido del archivo
                string contenido = $"Nombre: {datos.name}\n" +
                                   $"Peso: {datos.weight}\n" +
                                   $"Altura: {datos.height}\n" +
                                   $"Habilidades:\n";

                foreach (var habilidad in datos.abilities)
                {
                    contenido += $"- {habilidad.ability.name}\n";
                }

                // Guardar el contenido en el archivo
                File.WriteAllText(rutaArchivo, contenido);

                MessageBox.Show($"Resultados guardados en: {rutaArchivo}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al guardar el archivo: {ex.Message}");
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtNombre.Clear();
            lblNombre.Text = "Nombre:";
            lblPeso.Text = "Peso:";
            lblAltura.Text = "Altura:";
            pictureBox1.Image = null;
            lstHabilidades.Items.Clear();
        }

        private void lstHabilidades_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    public class Pokemon
    {
        public string name { get; set; }
        public int weight { get; set; }
        public int height { get; set; }
        public Sprites sprites { get; set; }
        public List<AbilityWrapper> abilities { get; set; }
    }

    public class AbilityWrapper
    {
        public Ability ability { get; set; }
    }

    public class Ability
    {
        public string name { get; set; }
    }

    public class Sprites
    {
        public string front_default { get; set; }
    }
}
