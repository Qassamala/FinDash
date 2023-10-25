import React, { useState } from 'react';
import axios from 'axios';

export default function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = async () => {
    try {
      const response = await axios.post('https://localhost:7222/Auth/token', {
        username,
        password,
      });
      const token = response.data.token;
      const isAdmin = response.data.isAdmin;
      const userId = response.data.id;
      localStorage.setItem('token', token);
      localStorage.setItem('isAdmin', isAdmin);
      localStorage.setItem('userId', userId); // Store the user ID
      window.location.href = isAdmin ? '/admin' : '/home';
    } catch (error) {
      console.error('Login failed:', error);
    }
  };

  return (
    <div className="login-container">
      <div className="login-header">
        <img src="./logo333.png" alt="Logo" className="login-logo" />
      </div>
      <h1>FinDash - a financial dashboard</h1>
      <div className="login-content">
        <div className="input-group">
          <label>Username</label>
          <input 
            type="text" 
            placeholder="Username" 
            onChange={(e) => setUsername(e.target.value)} 
          />
        </div>
        <div className="input-group">
          <label>Password</label>
          <input 
            type="password" 
            placeholder="Password" 
            onChange={(e) => setPassword(e.target.value)} 
          />
        </div>
        <button className="login-button" onClick={handleLogin}>LOG IN</button>
        {/* If you want to add the 'Remember me' and 'Forgot password?' functionalities later, you can include them here */}
      </div>
       <div className="login-footer"> 
       {/* <div className="support-info">
          Contact support: <br />
          Phone: Your phone number <br />
          E-mail: Your email address
        </div>*/}
        <div className="copyright-info">
          Copyright &copy; 2023. Nacky Software AB. All rights reserved.
        </div>
      </div>
    </div>
  );
}
