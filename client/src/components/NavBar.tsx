import {Link} from "react-router";
import useJerneCrud from "../hooks/useJerneCrud.ts";
import {useAtom} from "jotai";
import {loggedInUserAtom} from "../atoms/jerneAtom.ts";

export default function NavBar() {
    const jerneCrud = useJerneCrud();

    const [loggedInUser] = useAtom(loggedInUserAtom);

    return (
        <div className="navbar bg-base-100 shadow-sm">
            <div className="navbar-start">
                <div className="dropdown">
                    <div tabIndex={0} role="button" className="btn btn-ghost lg:hidden">
                        <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"> <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 6h16M4 12h8m-8 6h16" /> </svg>
                    </div>
                    <ul
                        tabIndex={0}
                        className="menu menu-sm dropdown-content bg-base-100 rounded-box z-1 mt-3 w-52 p-2 shadow">
                        <li><Link to='/'>Books</Link></li>
                        <li><Link to='/authors'>Authors</Link></li>
                        <li><Link to='/genres'>Genres</Link></li>
                    </ul>
                </div>
                <a className="btn btn-ghost text-xl">LIBRARY</a>
            </div>
            <div className="navbar-center hidden lg:flex">
                <ul className="menu menu-horizontal px-1">
                    <li><Link to='/'>Books</Link></li>
                    <li><Link to='/authors'>Authors</Link></li>
                    <li><Link to='/genres'>Genres</Link></li>
                </ul>
            </div>
            <div className="navbar-end">
                <div className="dropdown dropdown-end">
                    <div tabIndex={0} role="button" className="btn m-1 flex items-center">{loggedInUser?.fullName} ▼</div>
                    <ul tabIndex="-1" className="dropdown-content menu bg-base-100 rounded-box z-1 w-52 p-2 shadow-sm">
                        <li><a>Item 1</a></li>
                        <li><a onClick={() => {
                            jerneCrud.logoutUser()
                        }}>Logout</a></li>
                    </ul>
                </div>
            </div>
        </div>
    )
}