namespace TrafikCezaYonetimi.Web.Models;

public class ErrorViewModel
{
    // ErrorViewModel, uygulamada oluşan hataları kullanıcıya göstermek için kullanılan bir modeldir. RequestId, hatanın hangi istek sırasında oluştuğunu takip etmek için kullanılır.  
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
