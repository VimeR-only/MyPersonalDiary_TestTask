import React, { useState, useEffect } from 'react';

function Home({ username, onLogout }) {
  const [entries, setEntries] = useState([]);
  const [newEntry, setNewEntry] = useState('');
  const [image, setImage] = useState(null);
  const [search, setSearch] = useState('');
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');
  const [isAdmin, setIsAdmin] = useState(false);
  const [inviteEmail, setInviteEmail] = useState('');
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [isDeleted, setIsDeleted] = useState(false);

  useEffect(() => {
    const checkAccountExists = async () => {
      const res = await fetch('https://localhost:7018/api/User/exists', {
        credentials: 'include'
      });

      if (res.ok) {
        const data = await res.json();
        if (!data.exists) {
          onLogout();
        }
      } else {
        onLogout();
      }
    };

    checkAccountExists();
  }, []);

  const pageSize = 5;

  const fetchFilteredEntries = async () => {
    const params = new URLSearchParams();

    if (search) params.append('text', search);
    if (fromDate) params.append('from', fromDate + "T00:00:00");
    if (toDate) params.append('to', toDate + "T23:59:59");

    params.append('page', page);
    params.append('pageSize', pageSize);

    const res = await fetch(`https://localhost:7018/api/record/search?${params.toString()}`, { credentials: 'include', });

    if (res.ok) {
      const data = await res.json();
      setEntries(data);
      setHasMore(data.length === pageSize);
    }
  };

  useEffect(() => {
    const checkAccountDeletionStatus = async () => {
      const res = await fetch('https://localhost:7018/api/user/deletion-status', { credentials: 'include' });
      
      if (res.ok) {
        const data = await res.json();

        setIsDeleted(data.isDeleted);
      }
    };
    checkAccountDeletionStatus();
  }, []);

  useEffect(() => {
    if (!isDeleted) {
      fetchFilteredEntries();
    }
  }, [search, fromDate, toDate, page, isDeleted]);

  useEffect(() => {
    const checkAdmin = async () => {
      const res = await fetch('https://localhost:7018/api/auth/is-admin', { credentials: 'include' });

      if (res.ok) {
        const data = await res.json();

        setIsAdmin(data.isAdmin);
      }
    };
    checkAdmin();
  }, []);

  const addEntry = async (e) => {
    e.preventDefault();

    const formData = new FormData();
    formData.append('content', newEntry);
    if (image) {
      formData.append('image', image);
    }

    const res = await fetch('https://localhost:7018/api/Record', {
      method: 'POST',
      body: formData,
      credentials: 'include'
    });

    if (res.ok) {
      setNewEntry('');
      setImage(null);
      setPage(1);
      await fetchFilteredEntries();
    }
  };

  const deleteEntry = async (publicId) => {
    const res = await fetch(`https://localhost:7018/api/Record/${publicId}`, {
      method: 'DELETE',
      credentials: 'include'
    });

    if (res.ok) {
      await fetchFilteredEntries();
    }
  };

  const handleInvite = async () => {
    if (!inviteEmail.includes('@')) return alert("Incorrect mail.");

    const res = await fetch('https://localhost:7018/api/admin/invites/create-and-send', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({ email: inviteEmail })
    });

    if (res.ok) {
      alert("Invitation sent.");

      setInviteEmail('');
    }
  };

  const deleteAccount = async () => {
    if (!window.confirm("Are you sure you want to delete your account? Your data will be deleted after 2 days.")) return;

    const res = await fetch('https://localhost:7018/api/user/delete', { method: 'POST', credentials: 'include' });

    if (res.ok) {
      alert("Your account will be deleted in 2 days. You can restore your account before then.");

      setIsDeleted(true);
    } else {
      alert("Error deleting account.");
    }
  };

  const restoreAccount = async () => {
    const res = await fetch('https://localhost:7018/api/user/restore', { method: 'POST', credentials: 'include' });

    if (res.ok) {
      alert("Account restored.");

      setIsDeleted(false);

      await fetchFilteredEntries();

    } else {
      alert("Account recovery failed.");
    }
  };

  if (isDeleted) {
    return (
      <div style={{ padding: 20, textAlign: 'center', marginTop: 100 }}>
        <h2>Your account has been marked for deletion.</h2>
        <p>You can restore your account within 2 days.</p>
        <button onClick={restoreAccount} style={{ marginRight: 10 }}>Recover account</button>
        <button onClick={onLogout}>Exit</button>
      </div>
    );
  }

  return (
    <div style={{ padding: 20 }}>
      <button onClick={onLogout}>Exit</button>

      <form onSubmit={addEntry} style={{ marginTop: 20 }}>
        <textarea
          value={newEntry}
          onChange={(e) => setNewEntry(e.target.value)}
          placeholder="Write ..."
          rows={5}
          cols={50}
          maxLength={500}
        />
        <br />
        <input type="file" onChange={(e) => setImage(e.target.files[0])} />
        <br />
        <button type="submit">Add entry</button>
      </form>

      <div style={{ marginTop: 20 }}>
        <input
          type="text"
          placeholder="Text search"
          value={search}
          onChange={(e) => { setSearch(e.target.value); setPage(1); }}
        />
        <br />
        <label>Від: </label>
        <input
          type="date"
          value={fromDate}
          onChange={(e) => { setFromDate(e.target.value); setPage(1); }}
        />
        <label> До: </label>
        <input
          type="date"
          value={toDate}
          onChange={(e) => { setToDate(e.target.value); setPage(1); }}
        />
      </div>

      <div style={{ marginTop: 20 }}>
        {entries.map(entry => (
          <div key={entry.publicId} style={{ border: '1px solid #ccc', padding: 10, marginBottom: 10 }}>
            <p>{entry.content}</p>
            {entry.imagePath && (
              <img
                src={`https://localhost:7018${entry.imagePath.replace("wwwroot", "")}`}
                alt="img"
                style={{ maxWidth: 200 }}
              />
            )}
            <p><i>{new Date(entry.createdAt).toLocaleString()}</i></p>
            <button onClick={() => deleteEntry(entry.publicId)}>Remove</button>
          </div>
        ))}
      </div>

      <div style={{ marginTop: 10 }}>
        <button onClick={() => setPage(p => Math.max(p - 1, 1))} disabled={page === 1}>← Back</button>
        <span style={{ margin: '0 10px' }}>Page {page}</span>
        <button onClick={() => setPage(p => p + 1)} disabled={!hasMore}>Forward →</button>
      </div>

      {isAdmin && (
        <div style={{ marginTop: 30 }}>
          <h3>Invite user</h3>
          <input
            type="email"
            placeholder="Email"
            value={inviteEmail}
            onChange={(e) => setInviteEmail(e.target.value)}
          />
          <button onClick={handleInvite}>Send</button>
        </div>
      )}

      <button onClick={deleteAccount} style={{ marginTop: 30, backgroundColor: 'red', color: 'white', padding: '10px 20px' }} >
        Delete account
      </button>
    </div>
  );
}

export default Home;