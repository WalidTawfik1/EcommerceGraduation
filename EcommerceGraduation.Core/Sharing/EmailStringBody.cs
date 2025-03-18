using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Sharing
{
    public class EmailStringBody
    {
        public static string send(string email, string token, string component, string message)
        {
            string encodeToken = Uri.EscapeDataString(token);
            return $@"
                        <html>
                    <head>
                        <style>
                            .container{{
                                width: 100%;
                                height: 100%;
                                display: flex;
                                justify-content: center;
                                align-items: center;
                                background-color: #f1f1f1;
                            }}
                            .content{{
                                width: 50%;
                                height: 50%;
                                background-color: white;
                                padding: 20px;
                                border-radius: 10px;
                                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                            }}
                            .message{{
                                font-size: 16px;
                                margin-bottom: 20px;
                            }}
                            .button{{
                                padding: 10px 20px;
                                background-color: #007bff;
                                color: white;
                                text-decoration: none;
                                border-radius: 5px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='content'>
                                <div class='message'>{message}</div>
                                <a href=""http://localhost:3000/Account/component?email={email}&code={encodeToken}""> 
                                        {message} 
                                    </a>
                            </div>
                        </div>
                    </body>
                </html>";
        }
    }
}
