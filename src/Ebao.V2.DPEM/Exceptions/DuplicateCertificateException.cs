namespace Ebao.V2.DPEM.Exceptions;

public class DuplicateCertificateException : Exception
{
    public string Certificate { get; set; }

    public DuplicateCertificateException(string certificate)
    {
        Certificate = certificate;
    }
}