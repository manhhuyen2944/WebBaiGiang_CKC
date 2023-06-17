using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebBaiGiang_CKC.Models
{
    public class MonHoc
    {
        public int MonHocId { get; set; }
        [DisplayName("Tên môn học")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public string TenMonHoc { get; set; }
        [DisplayName("Mã môn học")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public string MaMonHoc { get; set; }
        [DisplayName("Giới thiệu môn học")]
        [Column(TypeName = "ntext")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public string MoTa { get; set; }
        public List<Chuong> Chuongs { get; set; }
    }
}
