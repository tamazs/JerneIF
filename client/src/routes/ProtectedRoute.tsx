import { Navigate } from "react-router";

// @ts-ignore
export default function ProtectedRoute({ children }) {
    const token = localStorage.getItem("accessToken");

    if (!token) return <Navigate to="/login" replace />;

    return children;
}
