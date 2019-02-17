namespace PhotoChronLib
{
    public interface IPhotoRenamingService
    {
        void AddFilePath(string path);
        void RenameImagesByDateTaken();
    }
}
