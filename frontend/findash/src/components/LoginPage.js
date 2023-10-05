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
      console.log(userId)
      console.log(userId)
      localStorage.setItem('token', token);
      localStorage.setItem('isAdmin', isAdmin);
      localStorage.setItem('userId', userId); // Store the user ID
      window.location.href = isAdmin ? '/admin' : '/home';
    } catch (error) {
      console.error('Login failed:', error);
    }
  };

  return (
    <div id='loginDiv'>
       <div>
           <div id='appName'>FinDash App</div>
            <div>
                <input placeholder="Username" onChange={(e) => setUsername(e.target.value)} />
            </div>
            <div>
                <input placeholder="Password" type="password" onChange={(e) => setPassword(e.target.value)} />
            </div>
            <button onClick={handleLogin}>Login</button>
        </div>
      
    </div>
  );
}
