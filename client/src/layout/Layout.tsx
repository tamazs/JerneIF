import NavBar from "../components/NavBar.tsx";
import {Outlet} from "react-router";

export default function Layout() {
    return (
        <>
            <NavBar />
            <main className="main">
                <Outlet />
            </main>
        </>
    )
}