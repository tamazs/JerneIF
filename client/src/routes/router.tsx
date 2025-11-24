import {createBrowserRouter} from "react-router";
import Layout from "../layout/Layout.tsx";
import ProtectedRoute from "./ProtectedRoute.tsx";
import PlayerDashboard from "../pages/admin/PlayerDashboard.tsx";
import AdminDashboard from "../pages/player/AdminDashboard.tsx";
import LoginPage from "../pages/LoginPage.tsx";
import Game from "../pages/player/Game.tsx";

export const router = createBrowserRouter([
    {
        path: "/",
        element: (
            <ProtectedRoute>
                <Layout />
            </ProtectedRoute>
        ),
        children: [
            {
                index: true,
                element: <PlayerDashboard />
            },
            {
                path: "admin",
                element: <AdminDashboard />
            },
            {
                path: "game",
                element: <Game />
            }
        ]
    },
    {
        path: "/login",
        element: <LoginPage />
    },
    ]
);