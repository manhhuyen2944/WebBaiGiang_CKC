using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebBaiGiang_CKC.Models
{
    public class Chuong
    {
        public int ChuongId { get; set; }
        [DisplayName("Tên chương")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public string TenChuong { get; set; }
        [DisplayName("Chương số")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public int SoChuong { get; set; }

        [DisplayName("Tên môn học")]
        public int MonHocId { get; set; }

        public MonHoc MonHoc { get; set; }
        public List<Bai> Bais { get; set; }
    }
}
