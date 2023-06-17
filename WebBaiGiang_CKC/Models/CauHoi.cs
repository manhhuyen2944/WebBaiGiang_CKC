using System.ComponentModel;

namespace WebBaiGiang_CKC.Models
{
    public class CauHoi
    {
        public int CauHoiId { get; set; }

        [DisplayName("Tên chương")]
        public int ChuongId { get; set; }

        public Chuong Chuong { get; set; }
        [DisplayName("Câu hỏi")]
        public string NoiDung { get; set; }

        [DisplayName("Câu trả lời")]
        public string? DapAnA { get; set; }
        [DisplayName("Câu trả lời")]
        public string? DapAnB { get; set; }
        [DisplayName("Câu trả lời")]

        public string? DapAnC { get; set; }
        [DisplayName("Câu trả lời")]
        public string? DapAnD { get; set; }
        [DisplayName("Đáp án đúng")]
        public string DapAnDung { get; set; }

        [DisplayName("Độ khó")]
        public float DoKho { get; set; }
        public int? SoLanLay { get; set; }
        public int? SoLanTraLoiDung { get; set; }

        public virtual ICollection<CauHoi_De> CauHoi_De { get; set; }
    }
}
