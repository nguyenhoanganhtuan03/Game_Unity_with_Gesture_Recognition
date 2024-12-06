using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.IO;

public class UnityUDPClient : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    private Process pythonProcess;
    private bool pythonScriptStarted = false;
    private PlayerController playerController;
    private static UnityUDPClient instance;
    private bool isReceiving = false;

    void Awake()
    {
        // Đảm bảo chỉ có một instance duy nhất
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Không phá hủy khi chuyển scene
        }
    }

    void Start()
    {
        // Khởi tạo PlayerController
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            UnityEngine.Debug.Log("Không tìm thấy PlayerController trên GameObject này.");
        }

        // Khởi tạo UDP socket
        InitializeUDP();
        RunPythonScript();
    }

    private void InitializeUDP()
    {
        // Khởi tạo lại UdpClient nếu cần thiết
        if (udpClient == null)
        {
            udpClient = new UdpClient(5005);
            remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5005); // Sử dụng localhost và port 5005
            isReceiving = true;
            StartCoroutine(ReceiveDataCoroutine());
        }
    }

    // Coroutine để nhận dữ liệu liên tục
    private System.Collections.IEnumerator ReceiveDataCoroutine()
    {
        while (isReceiving)
        {
            ReceiveUDPData();
            yield return null;
        }
    }

    public void ReceiveUDPData()
    {
        try
        {
            if (udpClient.Available > 0)
            {
                byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                int gestureIndex = receivedBytes[0];

                // UnityEngine.Debug.Log("Nhận dữ liệu từ Python: " + gestureIndex);

                if (playerController == null)
                {
                    // Tìm lại PlayerController khi vào scene mới
                    playerController = FindObjectOfType<PlayerController>();
                }

                if (playerController != null)
                {
                    if (gestureIndex != 255)
                    {
                        playerController.ControlCharacter(gestureIndex);
                    }
                }
            }
        }
        catch (SocketException ex)
        {
            UnityEngine.Debug.LogError("Lỗi nhận dữ liệu UDP: " + ex.Message);
        }
    }

    private void OnApplicationQuit()
    {
        // Đóng UdpClient khi ứng dụng thoát
        if (udpClient != null)
        {
            isReceiving = false;
            udpClient.Close();
            UnityEngine.Debug.Log("UdpClient đã được đóng.");
        }

        // Tắt process Python nếu còn đang chạy
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            try
            {
                KillPythonProcess(pythonProcess.Id);
                UnityEngine.Debug.Log("Python process đã được tắt bằng taskkill.");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError("Lỗi khi tắt Python process: " + ex.Message);
            }
            finally
            {
                pythonProcess.Dispose();
            }
        }
    }

    private void KillPythonProcess(int pid)
    {
        // Sử dụng lệnh taskkill để tắt process và các subprocess
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c taskkill /PID {pid} /T /F",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using (Process taskkillProcess = Process.Start(startInfo))
        {
            taskkillProcess.WaitForExit();
            if (taskkillProcess.ExitCode != 0)
            {
                UnityEngine.Debug.LogError("Lỗi khi thực hiện taskkill: " + taskkillProcess.StandardError.ReadToEnd());
            }
            else
            {
                UnityEngine.Debug.Log("Đã tắt Python process thành công.");
            }
        }
    }

    private void RunPythonScript()
    {
        try
        {
            // Tạo đường dẫn đến file .exe
            //string scriptPath = Application.dataPath + "/" + "model_exe/" + "model_Mediapipe_5_labels.exe";
            //string scriptPath = Application.dataPath + "/" + "model_exe/" + "model_CNN_5_labels.exe";
            string scriptPath = Application.dataPath + "/Scripts/Python/gesture_UDP_CNN_5_labels.py";
            //string scriptPath = Application.dataPath + "/Scripts/Python/gesture_UDP_Mediapipe_5_labels.py";
            UnityEngine.Debug.Log("Đường dẫn đến file exe: " + scriptPath);

            // Kiểm tra xem file có tồn tại không
            if (!File.Exists(scriptPath))
            {
                UnityEngine.Debug.LogError("File không tồn tại: " + scriptPath);
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\"",
                //FileName = scriptPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            pythonProcess = new Process { StartInfo = startInfo };

            pythonProcess.OutputDataReceived += (sender, e) =>
            {
                // Không làm gì ở đây để không in thông tin ra
            };

            pythonProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    UnityEngine.Debug.LogError("Python Error: " + e.Data);
                }
            };

            pythonProcess.Start();
            pythonProcess.BeginOutputReadLine();
            pythonProcess.BeginErrorReadLine();

            pythonScriptStarted = true;
            UnityEngine.Debug.Log("Python script đã bắt đầu.");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("Lỗi khi chạy Python script: " + ex.Message);
            pythonScriptStarted = false;
        }
    }
}