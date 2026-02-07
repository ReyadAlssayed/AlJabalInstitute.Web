window.studentLogin = async (nationalId, password) => {
    const resp = await fetch("/api/student/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ nationalId, password })
    });

    const text = await resp.text();
    return { ok: resp.ok, text };
};
