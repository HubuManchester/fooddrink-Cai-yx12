using Microsoft.Maui.ApplicationModel;

namespace RecipeRandomizer.Services;

public static class CameraService
{
    public static async Task<string?> TakePhotoAsync()
    {
        try
        {
            // 请求相机权限
            var cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
            if (cameraStatus != PermissionStatus.Granted)
            {
                return null;
            }

            // 请求存储权限（Android 13 以下需要）
            if (DeviceInfo.Current.Platform == DevicePlatform.Android && DeviceInfo.Version.Major < 13)
            {
                var storageStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
                if (storageStatus != PermissionStatus.Granted)
                {
                    // 存储权限不是必须的，只是保存照片需要
                }
            }

            // 使用 MediaPicker 拍照
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo == null)
                return null;

            // 保存到本地缓存
            var localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using var sourceStream = await photo.OpenReadAsync();
            using var fileStream = File.OpenWrite(localFilePath);
            await sourceStream.CopyToAsync(fileStream);

            return localFilePath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Camera error: {ex.Message}");
            return null;
        }
    }
}