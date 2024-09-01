//using VimeoDotNet.Net;

//namespace Courses_app.Models
//{
//    public class FileBinaryContent : IBinaryContent
//    {
//        public FileBinaryContent(FileInfo fileInfo)
//        {
//            Data = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
//            OriginalFileName = fileInfo.Name;
//        }

//        public Stream Data { get; }
//        public string OriginalFileName { get; }
//        public string ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
//        string IBinaryContent.OriginalFileName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//        public Task<byte[]> ReadAllAsync()
//        {
//            throw new NotImplementedException();
//        }

//        public Task<byte[]> ReadAsync(long startIndex, long endIndex)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
