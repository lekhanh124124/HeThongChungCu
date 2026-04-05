namespace HeThongChungCu.WebAPI.Common.Templates;

public static class IdentityHtmlTemplates
{
    public static string GetIdentificationProcessingPage(string token, string postUrl)
    {
        return $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Xác thực danh tính | Hệ thống Chung cư</title>
    <link rel=""preconnect"" href=""https://fonts.googleapis.com"">
    <link rel=""preconnect"" href=""https://fonts.gstatic.com"" crossorigin>
    <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@300;400;600;700&family=Outfit:wght@500;700&display=swap"" rel=""stylesheet"">
    <style>
        :root {{
            --primary: #4F46E5;
            --secondary: #7C3AED;
            --accent: #10B981;
            --error: #EF4444;
            --bg: #0F172A;
            --glass: rgba(255, 255, 255, 0.05);
            --glass-border: rgba(255, 255, 255, 0.1);
        }}

        * {{ box-sizing: border-box; margin: 0; padding: 0; }}
        
        body {{
            font-family: 'Inter', sans-serif;
            background-color: var(--bg);
            background-image: 
                radial-gradient(at 0% 0%, rgba(79, 70, 229, 0.15) 0px, transparent 50%),
                radial-gradient(at 100% 100%, rgba(124, 58, 237, 0.15) 0px, transparent 50%);
            color: #F8FAFC;
            height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
        }}

        .container {{
            width: 100%;
            max-width: 450px;
            padding: 2rem;
            text-align: center;
            z-index: 10;
        }}

        .glass-panel {{
            background: var(--glass);
            backdrop-filter: blur(12px);
            -webkit-backdrop-filter: blur(12px);
            border: 1px solid var(--glass-border);
            border-radius: 24px;
            padding: 3rem 2rem;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
            transform: translateY(20px);
            opacity: 0;
            animation: slideUp 0.8s cubic-bezier(0.16, 1, 0.3, 1) forwards;
        }}

        @keyframes slideUp {{
            to {{ transform: translateY(0); opacity: 1; }}
        }}

        .logo {{
            font-family: 'Outfit', sans-serif;
            font-size: 1.5rem;
            font-weight: 700;
            margin-bottom: 2rem;
            background: linear-gradient(to right, #818CF8, #C084FC);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
        }}

        .status-header {{
            font-size: 1.75rem;
            font-weight: 700;
            margin-bottom: 1rem;
            color: #FFFFFF;
        }}

        .status-desc {{
            font-size: 1rem;
            color: #94A3B8;
            line-height: 1.6;
            margin-bottom: 2.5rem;
        }}

        /* Loading Spinner */
        .loader {{
            width: 64px;
            height: 64px;
            border: 4px solid var(--glass-border);
            border-top: 4px solid var(--primary);
            border-radius: 50%;
            margin: 0 auto 2rem;
            animation: spin 1s linear infinite;
        }}

        @keyframes spin {{ 0% {{ transform: rotate(0deg); }} 100% {{ transform: rotate(360deg); }} }}

        /* Icon Styles */
        .icon-wrapper {{
            display: none;
            width: 80px;
            height: 80px;
            border-radius: 50%;
            margin: 0 auto 2rem;
            align-items: center;
            justify-content: center;
            font-size: 2.5rem;
        }}

        .icon-success {{
            background: rgba(16, 185, 129, 0.1);
            color: var(--accent);
            border: 2px solid rgba(16, 185, 129, 0.2);
        }}

        .icon-error {{
            background: rgba(239, 68, 68, 0.1);
            color: var(--error);
            border: 2px solid rgba(239, 68, 68, 0.2);
        }}

        .btn {{
            display: none;
            width: 100%;
            padding: 1rem;
            border-radius: 12px;
            border: none;
            background: linear-gradient(to right, var(--primary), var(--secondary));
            color: white;
            font-weight: 600;
            font-size: 1rem;
            cursor: pointer;
            transition: all 0.3s ease;
            text-decoration: none;
            margin-top: 1rem;
        }}

        .btn:hover {{
            transform: translateY(-2px);
            box-shadow: 0 10px 20px -5px rgba(79, 70, 229, 0.4);
        }}

        /* Progress Dots */
        .dots {{ display: inline-block; width: 24px; text-align: left; }}
        .dots::after {{
            content: '.';
            animation: dots 1.5s steps(5, end) infinite;
        }}
        @keyframes dots {{
            0%, 20% {{ content: '.'; }}
            40% {{ content: '..'; }}
            60% {{ content: '...'; }}
            80%, 100% {{ content: ''; }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""glass-panel"" id=""mainPanel"">
            <div class=""logo"">HỆ THỐNG CHUNG CƯ</div>
            
            <div id=""loader"" class=""loader""></div>
            <div id=""successIcon"" class=""icon-wrapper icon-success"">✓</div>
            <div id=""errorIcon"" class=""icon-wrapper icon-error"">!</div>

            <h1 id=""statusTitle"" class=""status-header"">Đang xác thực</h1>
            <p id=""statusMessage"" class=""status-desc"">Vui lòng chờ trong giây lát, chúng tôi đang kiểm tra mã định danh của bạn<span class=""dots""></span></p>

            <a href=""#"" id=""primaryBtn"" class=""btn"">Quay lại ứng dụng</a>
        </div>
    </div>

    <script>
        const token = '{token}';
        const postUrl = '{postUrl}';

        async function verify() {{
            try {{
                const response = await fetch(postUrl, {{
                    method: 'POST',
                    headers: {{
                        'Content-Type': 'application/json'
                    }},
                    body: JSON.stringify({{ token: token }})
                }});

                const result = await response.json();
                console.log('Verification Result:', result);

                if (response.ok && result.isOk) {{
                    showSuccess();
                }} else {{
                    const errorDescription = (result.errors && result.errors.length > 0) 
                        ? result.errors[0].description 
                        : 'Có lỗi xảy ra trong quá trình xác thực. Vui lòng thử lại sau.';
                    showError(errorDescription);
                }}
            }} catch (err) {{
                showError('Không thể kết nối đến máy chủ. Vui lòng kiểm tra kết nối mạng.');
            }}
        }}

        function showSuccess() {{
            document.getElementById('loader').style.display = 'none';
            document.getElementById('successIcon').style.display = 'flex';
            document.getElementById('statusTitle').innerText = 'Thành công!';
            document.getElementById('statusTitle').style.color = '#10B981';
            document.getElementById('statusMessage').innerText = 'Tài khoản của bạn đã được xác thực thành công. Bạn hiện có thể đăng nhập vào hệ thống.';
            const btn = document.getElementById('primaryBtn');
            btn.style.display = 'block';
            btn.innerText = 'Đăng nhập ngay';
            btn.href = 'https://portal.chungcu.com/login'; // Adjust this URL as needed
        }}

        function showError(msg) {{
            document.getElementById('loader').style.display = 'none';
            document.getElementById('errorIcon').style.display = 'flex';
            document.getElementById('statusTitle').innerText = 'Không thành công';
            document.getElementById('statusTitle').style.color = '#EF4444';
            document.getElementById('statusMessage').innerText = msg;
            const btn = document.getElementById('primaryBtn');
            btn.style.display = 'block';
            btn.innerText = 'Về trang chủ';
            btn.style.background = '#334155';
            btn.href = '/';
        }}

        // Start verification after a short delay for smooth UI feel
        setTimeout(verify, 1500);
    </script>
</body>
</html>
";
    }
}
