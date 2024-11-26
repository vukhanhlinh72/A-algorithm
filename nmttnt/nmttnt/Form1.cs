using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nmttnt
{
    public partial class Form1 : Form
    {
        List<PictureBox> pictureboxlist = new List<PictureBox>();
        List<Bitmap> images = new List<Bitmap>();
        List<string> locations = new List<string>();
        List<string> current_locations = new List<string>();

        string win_positon;
        string current_postion;

        Bitmap MainBitmap;
        private string win_position;

        public Form1()
        {
            InitializeComponent();
            but_rs.Click += new EventHandler(ShuffleImages);
        }

        // Open File để lấy ảnh
        private void OpenFileEvent(object sender, EventArgs e)
        {
            if (pictureboxlist != null)
            {

                foreach (PictureBox pics in pictureboxlist.ToList())
                {
                    this.Controls.Remove(pics);
                }

                pictureboxlist.Clear();
                images.Clear();
                locations.Clear();
                current_locations.Clear();
                win_positon = string.Empty;
                current_postion = string.Empty;
                lable2.Text = string.Empty;
            }

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files Only | *.jpg; *.jpeg; *.gif; *.png";

            if (open.ShowDialog() == DialogResult.OK)
            {

                MainBitmap = new Bitmap(open.FileName);
                CreatePictureBoxes();
                AddImagde();
            }
           
            if (open.ShowDialog() == DialogResult.OK)
            {
                // ...
                AddImagde();

                // Xáo trộn ảnh khi mở một hình mới
                ShuffleImages(sender, e);
            }
        }


        // Trộn ảnh
        private void ShuffleImages(object sender, EventArgs e)
        {
            Random random = new Random();

            // Trộn ảnh bằng cách hoán đổi vị trí với ô trống nhiều lần
            for (int i = 0; i < 100; i++)
            {
                int emptyIndex = current_locations.IndexOf("empty");
                List<int> adjacentIndexes = new List<int>();

                // Tìm các ô có thể di chuyển đến ô trống
                for (int j = 0; j < current_locations.Count; j++)
                {
                    if (IsAdjacent(j, emptyIndex))
                    {
                        adjacentIndexes.Add(j);
                    }
                }

                if (adjacentIndexes.Count > 0)
                {
                    // Chọn ngẫu nhiên một ô có thể di chuyển
                    int randomAdjacentIndex = adjacentIndexes[random.Next(adjacentIndexes.Count)];

                    // Hoán đổi vị trí của ô được chọn và ô trống
                    string temp = current_locations[randomAdjacentIndex];
                    current_locations[randomAdjacentIndex] = current_locations[emptyIndex];
                    current_locations[emptyIndex] = temp;

                    // Di chuyển PictureBox tương ứng
                    SwapPictureBoxes(pictureboxlist[randomAdjacentIndex], pictureboxlist[emptyIndex]);

                }
            }
        }

        // Kiểm tra xem có nằm cạnh nhau không
        private bool IsAdjacent(int clickedIndex, int emptyIndex)
        {
            int clickedRow = clickedIndex / 3;
            int clickedCol = clickedIndex % 3;
            int emptyRow = emptyIndex / 3;
            int emptyCol = emptyIndex % 3;

            // Kiểm tra xem hai ô có nằm cạnh nhau (theo chiều ngang hoặc chiều dọc) hay không
            return (Math.Abs(clickedRow - emptyRow) == 1 && clickedCol == emptyCol) ||
                   (Math.Abs(clickedCol - emptyCol) == 1 && clickedRow == emptyRow);
        }

        //Hoán đổi vị trí
        private void SwapPictureBoxes(PictureBox pic1, PictureBox pic2)
        {
            // Hoán đổi vị trí và vị trí Z để đảm bảo hiển thị đúng trên Form
            Point tempLocation = pic1.Location;
            pic1.Location = pic2.Location;
            pic2.Location = tempLocation;

            int tempZ = pic1.TabIndex;
            pic1.TabIndex = pic2.TabIndex;
            pic2.TabIndex = tempZ;

            // Hoán đổi Tag để cập nhật vị trí
            object tempTag = pic1.Tag;
            pic1.Tag = pic2.Tag;
            pic2.Tag = tempTag;
        }



        private int clickCount = 0;



        //Tạo size ảnh
        private void CreatePictureBoxes()
        {
            for (int i = 0; i < 9; i++)
            {
                PictureBox temp_pic = new PictureBox();
                temp_pic.Size = new Size(130, 130);
                temp_pic.Tag = i.ToString();
                temp_pic.Click += OnPicClick;
                pictureboxlist.Add(temp_pic);
                locations.Add(temp_pic.Tag.ToString());
                temp_pic.Click += PictureBox_Click;
            }
                       
        }


        //Đếm số lần Click
        private void PictureBox_Click(object sender, EventArgs e)
        {
            // Tăng biến đếm khi PictureBox được click
            clickCount++;

            // Hiển thị số lần click trong TextBox txt_Count
            txt_Count.Text = clickCount.ToString();
        }

        // Hoán đổi, kiểm tra, di chuyển khi Click Image

        int[] positions = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, -1 };
        private void OnPicClick(object sender, EventArgs e)
        {
            PictureBox clickedPictureBox = (PictureBox)sender;
            int clickedIndex = Convert.ToInt32(clickedPictureBox.Tag);

            // Kiểm tra xem ô đó có thể di chuyển đến ô trống không
            if (CanMove(clickedIndex))
            {
                int emptyIndex = Array.IndexOf(positions, -1);

                // Hoán đổi vị trí giữa ô đó và ô trống trong mảng vị trí
                positions[emptyIndex] = positions[clickedIndex];
                positions[clickedIndex] = -1;

                // Chuyển động ô đó đến vị trí của ô trống
                SwapPictureBoxes(clickedPictureBox, pictureboxlist[emptyIndex]);

                // Kiểm tra xem trò chơi đã hoàn thành chưa
                if (IsGameCompleted())
                {
                    MessageBox.Show("Chúc mừng! Bạn đã hoàn thành trò chơi.");
                }
            }
        }

        // Di chuyển
        private bool CanMove(int clickedIndex)
        {
            int emptyIndex = Array.IndexOf(positions, -1);

            // Kiểm tra xem ô đó có thể di chuyển đến ô trống không
            if (clickedIndex == emptyIndex)
            {
                return false;
            }

            // Kiểm tra xem ô đó có cùng hàng hoặc cùng cột với ô trống không
            if (clickedIndex / 3 == emptyIndex / 3 || clickedIndex % 3 == emptyIndex % 3)
            {
                return true;
            }

            return false;
        }

        //Kiểm tra xem hoàn thành chưa
        private bool IsGameCompleted()
        {
            for (int i = 0; i < positions.Length - 1; i++)
            {
                if (positions[i] != i)
                {
                    return false;
                }
            }
            return true;
        }

        private void CropImage(Bitmap main_bitmap, int height, int width)
        {

            int x, y;
            x = 0;
            y = 0;

            for (int blocks = 0; blocks < 9; blocks++)
            {

                Bitmap cropped_image = new Bitmap(height, width);

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        cropped_image.SetPixel(i, j, main_bitmap.GetPixel((i + x), (j + y)));
                    }
                }
                images.Add(cropped_image);
                x += 130;
                if (x == 390)
                {
                    x = 0;
                    y += 130;
                }
            }

        }

        private void AddImagde()
        {
            Bitmap temBipmap = new Bitmap(MainBitmap, new Size(390, 390));

            CropImage(temBipmap, 130, 130);

            for (int i = 0; i < pictureboxlist.Count; i++)
            {
                pictureboxlist[i].BackgroundImage = (Image)images[i];
                this.Controls.Add(pictureboxlist[i]); // Thêm PictureBox vào Controls của Form
            }

            PlacePictureBoxesToForm();
        }

    

        private void PlacePictureBoxesToForm()
        {
            int x = 200;
            int y = 25;

            for(int i=0;i<pictureboxlist.Count; i++)
            {
                pictureboxlist[i].BackColor = Color.White;

                if( i ==3 || i== 6)
                {
                    y += 130;
                    x = 200;
                }

                pictureboxlist[i].BorderStyle = BorderStyle.FixedSingle;
                pictureboxlist[i].Location = new Point(x, y);   

                this.Controls.Add(pictureboxlist[i]);
                x += 130;

                win_position += locations[i];
            }

        }

        private void CheckGame()
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void but_rs_Click(object sender, EventArgs e)
        {
            but_rs.Click += new EventHandler(ResetImages);
            // Đặt giá trị của TextBox "txt_count" về 0 khi nhấn Button "but_rs"
            txt_Count.Text = "0";
            // Đặt lại biến đếm click về 0
            clickCount = 0;

        }
        private void ResetImages(object sender, EventArgs e)
        {
            // Đặt lại danh sách vị trí hiện tại cho các ô ảnh
            current_locations = Enumerable.Range(0, 9).Select(_ => "").ToList();
            current_locations[8] = "empty"; // Đánh dấu ô trống

            // Đặt lại ảnh của các ô ảnh về ảnh gốc
            for (int i = 0; i < pictureboxlist.Count; i++)
            {
                pictureboxlist[i].BackgroundImage = (Image)images[i];
            }

            // Đặt lại vị trí và hiển thị các ô ảnh
            PlacePictureBoxesToForm();
        }

        private void but_start_Click(object sender, EventArgs e)
        {
            ShuffleImages(null, null); // Truyền giá trị null cho tham số sender và EventArgs
                                       // Đặt giá trị của TextBox "txt_count" về 0 khi nhấn Button "but_start"
            txt_Count.Text = "0";
             // Đặt lại biến đếm click về 0
            clickCount = 0;
        }

        private void but_thoat_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Bạn có chắc muốn thoát ?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)== DialogResult.OK)
            {
                Application.Exit();
            }
        }
     
    }
}