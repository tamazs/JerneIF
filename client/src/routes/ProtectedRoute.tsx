import { Navigate } from "react-router";
import { useAtom } from "jotai";
import type {JSX} from "react";

import { loggedInUserAtom } from "../atoms/jerneAtom.ts";

export default function ProtectedRoute({
                                           children,
                                           role
                                       }: {
    children: JSX.Element;
    role?: string;
}) {

    const [loggedInUser] = useAtom(loggedInUserAtom);

    // Not logged in
    if (!loggedInUser) {
        return <Navigate to="/login" replace />;
    }

    // Role restricted
    if (role && loggedInUser.role !== role) {
        return <Navigate to="/" replace />;
    }

    return children;
}
