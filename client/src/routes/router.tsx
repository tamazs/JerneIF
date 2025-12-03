import {createBrowserRouter} from "react-router";
import Layout from "../layout/Layout.tsx";
import ProtectedRoute from "./ProtectedRoute.tsx";
import PlayerDashboard from "../pages/admin/PlayerDashboard.tsx";
import AdminDashboard from "../pages/player/AdminDashboard.tsx";
import LoginPage from "../pages/LoginPage.tsx";
import Game from "../pages/player/Game.tsx";
import UsersList from "../pages/admin/UsersList.tsx";
import AddUser from "../pages/admin/AddUser.tsx";
import EditUser from "../pages/admin/EditUser.tsx";
import AddTransaction from "../pages/player/AddTransaction.tsx";
import TransactionsPage from "../pages/admin/TransactionsPage.tsx";
import UserTransactionsPage from "../pages/player/UserTransactionsPage.tsx";

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
                element: (
                    <ProtectedRoute role="Admin">
                        <AdminDashboard />
                    </ProtectedRoute>
                )
            },
            {
                path: "userslist",
                element: (
                    <ProtectedRoute role="Admin">
                        <UsersList />
                    </ProtectedRoute>
                )
            },
            {
                path: "adduser",
                element: (
                    <ProtectedRoute role="Admin">
                        <AddUser />
                    </ProtectedRoute>
                )
            },
            {
                path: "edituser/:userId",
                element: (
                    <ProtectedRoute role="Admin">
                        <EditUser />
                    </ProtectedRoute>
                )
            },
            {
                path: "transactions",
                element: (
                    <ProtectedRoute role="Admin">
                        <TransactionsPage />
                    </ProtectedRoute>
                )
            },

            {
                path: "addtransaction",
                element: (
                    <ProtectedRoute role="Player">
                        <AddTransaction />
                    </ProtectedRoute>
                )
            },
            {
                path: "mytransactions",
                element: (
                    <ProtectedRoute role="Player">
                        <UserTransactionsPage />
                    </ProtectedRoute>
                )
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
]);