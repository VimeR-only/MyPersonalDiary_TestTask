import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';

function RegisterPage() {
  const { inviteId } = useParams();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [nickname, setNickname] = useState('');
  const [password, setPassword] = useState('');
  const [captchaAnswer, setCaptchaAnswer] = useState('');
  const [captchaQuestion, setCaptchaQuestion] = useState('');

  useEffect(() => {
    const fetchCaptcha = async () => {
      const res = await fetch('https://localhost:7018/api/auth/get-captcha', { credentials: 'include' });
      if (res.ok) {
        const data = await res.json();

        setCaptchaQuestion(`${data.firstNumber} + ${data.secondNumber} = ?`);
      }
    };

    fetchCaptcha();
  }, []);

    const handleRegister = async (e) => {
    e.preventDefault();

    const res = await fetch(`https://localhost:7018/api/auth/register/${inviteId}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({
            userName: nickname,
            email,
            password,
            captchaAnswer: parseInt(captchaAnswer)
        })
    });

    if (res. ok) {
        alert("Registration successful.");

        navigate("/login");
    } else {
        const error = await res.json();
        alert(error.message || "Error registration.");
    }
    };


  return (
    <div style={{ padding: 20 }}>
      <h2>Registration</h2>
      <form onSubmit={handleRegister}>
        <input
          type="text"
          placeholder="Nickname"
          value={nickname}
          onChange={(e) => setNickname(e.target.value)}
          required
        /><br />
        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        /><br />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        /><br />
        {captchaQuestion && (
          <>
            <label>{captchaQuestion}</label><br />
            <input
              type="number"
              value={captchaAnswer}
              onChange={(e) => setCaptchaAnswer(e.target.value)}
              required
            /><br />
          </>
        )}
        <button type="submit">Regist</button>
      </form>
    </div>
  );
}

export default RegisterPage;