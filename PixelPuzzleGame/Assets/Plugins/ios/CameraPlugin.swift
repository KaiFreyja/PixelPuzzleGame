import UIKit

// MARK: - 修正 UIImage 方向
extension UIImage {
    func fixedOrientation() -> UIImage {
        if imageOrientation == .up { return self }

        UIGraphicsBeginImageContextWithOptions(size, false, scale)
        draw(in: CGRect(origin: .zero, size: size))
        let normalizedImage = UIGraphicsGetImageFromCurrentImageContext()
        UIGraphicsEndImageContext()
        return normalizedImage ?? self
    }
}

// MARK: - 拍照 Delegate
class CameraDelegate: NSObject, UIImagePickerControllerDelegate, UINavigationControllerDelegate {
    static let shared = CameraDelegate()

    func imagePickerController(_ picker: UIImagePickerController,
                               didFinishPickingMediaWithInfo info: [UIImagePickerController.InfoKey : Any]) {
        picker.dismiss(animated: true)

        if let image = info[.originalImage] as? UIImage {
            // 修正方向
            let fixedImage = image.fixedOrientation()

            // 存到暫存路徑
            if let data = fixedImage.jpegData(compressionQuality: 0.8) {
                let filename = NSTemporaryDirectory().appending("unity_photo.jpg")
                try? data.write(to: URL(fileURLWithPath: filename))

                // 傳回 Unity
                DispatchQueue.main.async {
                    filename.withCString { cStr in
                        UnitySendMessage("Main", "OnPictureTaken", cStr)
                    }
                }
            }
        }
    }

    func imagePickerControllerDidCancel(_ picker: UIImagePickerController) {
        picker.dismiss(animated: true)
    }
}

// MARK: - UnitySendMessage 宣告
@_silgen_name("UnitySendMessage")
func UnitySendMessage(_ obj: UnsafePointer<CChar>, _ method: UnsafePointer<CChar>, _ msg: UnsafePointer<CChar>)

// MARK: - Unity 可呼叫函式
@_cdecl("takePicture")
public func takePicture() {
    DispatchQueue.main.async {
        guard let rootVC = UIApplication.shared.windows.first(where: { $0.isKeyWindow })?.rootViewController else { return }

        let picker = UIImagePickerController()
        picker.sourceType = .camera
        picker.delegate = CameraDelegate.shared
        rootVC.present(picker, animated: true)
    }
}

// 選取相簿圖片
@_cdecl("pickFromGallery")
public func pickFromGallery() {
    DispatchQueue.main.async {
        guard let rootVC = UIApplication.shared.windows.first(where: { $0.isKeyWindow })?.rootViewController else { return }

        let picker = UIImagePickerController()
        picker.sourceType = .photoLibrary
        picker.delegate = CameraDelegate.shared
        rootVC.present(picker, animated: true)
    }
}
