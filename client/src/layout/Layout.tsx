import {Outlet, useNavigate} from "react-router";
import useJerneCrud from "../hooks/useJerneCrud.ts";
import {useAtom} from "jotai";
import {balanceAtom, loggedInUserAtom} from "../atoms/jerneAtom.ts";
import logo from "../../public/logo.png";

export default function Layout() {
    const jerneCrud = useJerneCrud();

    const [balance] = useAtom(balanceAtom);

    const [loggedInUser] = useAtom(loggedInUserAtom);

    const navigate = useNavigate();

    const homePageNav = () => {
        if (loggedInUser?.role === "Admin") {
            navigate("/admin");
        } else {
            navigate("/");
        }
    }
    return (
        <>
            <div className="drawer lg:drawer-open">
                <input id="my-drawer-4" type="checkbox" className="drawer-toggle" />
                <div className="drawer-content">
                    {/* Navbar */}
                    <nav className="navbar w-full bg-primary flex items-center justify-between">
                        <div className="flex items-center">
                            <label htmlFor="my-drawer-4" aria-label="open sidebar" className="btn btn-square btn-ghost">
                                {/* Sidebar toggle icon */}
                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" strokeLinejoin="round" strokeLinecap="round" strokeWidth="1.5" fill="none" stroke="currentColor" className="my-1.5 inline-block size-6"><path d="M4 4m0 2a2 2 0 0 1 2 -2h12a2 2 0 0 1 2 2v12a2 2 0 0 1 -2 2h-12a2 2 0 0 1 -2 -2z"></path><path d="M9 4v16"></path><path d="M14 10l2 2l-2 2"></path></svg>
                            </label>
                            <div className="px-4">
                                <img src={logo} alt="logo" className="w-14 h-14" />
                            </div>
                            <div className="px-4">Dead pigeons - Jerne IF</div>
                        </div>
                        <div className="px-4">
                            <button className="btn btn-secondary text-primary">{loggedInUser?.fullName}</button>
                            <button className="btn btn-secondary text-primary">Balance: {balance} DKK</button>
                        </div>
                    </nav>
                    {/* Page content */}
                    <div className="p-4"><Outlet/></div>
                </div>

                <div className="drawer-side is-drawer-close:overflow-visible">
                    <label htmlFor="my-drawer-4" aria-label="close sidebar" className="drawer-overlay"></label>
                    <div className="flex min-h-full flex-col items-start bg-base-200 is-drawer-close:w-14 is-drawer-open:w-64">
                        {/* Sidebar content */}
                        <ul className="menu w-full grow gap-2">
                            {/* List item */}
                            <li>
                                <button className="is-drawer-close:tooltip is-drawer-close:tooltip-right" data-tip="Homepage" onClick={homePageNav}>
                                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                                         stroke-width="1.5" stroke="currentColor" className="size-6">
                                        <path stroke-linecap="round" stroke-linejoin="round"
                                              d="m2.25 12 8.954-8.955c.44-.439 1.152-.439 1.591 0L21.75 12M4.5 9.75v10.125c0 .621.504 1.125 1.125 1.125H9.75v-4.875c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21h4.125c.621 0 1.125-.504 1.125-1.125V9.75M8.25 21h8.25"/>
                                    </svg>

                                    <span className="is-drawer-close:hidden">Homepage</span>
                                </button>
                            </li>

                            <li>
                                <button className="is-drawer-close:tooltip is-drawer-close:tooltip-right" data-tip="Profile" onClick={() => {
                                    navigate(`/edituser/${loggedInUser?.userId}`);
                                }}>
                                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" className="size-6">
                                        <path strokeLinecap="round" strokeLinejoin="round" d="M17.982 18.725A7.488 7.488 0 0 0 12 15.75a7.488 7.488 0 0 0-5.982 2.975m11.963 0a9 9 0 1 0-11.963 0m11.963 0A8.966 8.966 0 0 1 12 21a8.966 8.966 0 0 1-5.982-2.275M15 9.75a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
                                    </svg>

                                    <span className="is-drawer-close:hidden">{loggedInUser?.fullName}</span>
                                </button>
                            </li>

                            <li>
                                <button className="is-drawer-close:tooltip is-drawer-close:tooltip-right" data-tip="Log out" onClick={() => jerneCrud.logoutUser()}>
                                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" stroke-width="1.5" fill="currentColor" className="size-6">
                                        <path fillRule="evenodd" d="M17 4.25A2.25 2.25 0 0 0 14.75 2h-5.5A2.25 2.25 0 0 0 7 4.25v2a.75.75 0 0 0 1.5 0v-2a.75.75 0 0 1 .75-.75h5.5a.75.75 0 0 1 .75.75v11.5a.75.75 0 0 1-.75.75h-5.5a.75.75 0 0 1-.75-.75v-2a.75.75 0 0 0-1.5 0v2A2.25 2.25 0 0 0 9.25 18h5.5A2.25 2.25 0 0 0 17 15.75V4.25Z" clipRule="evenodd" />
                                        <path fillRule="evenodd" d="M14 10a.75.75 0 0 0-.75-.75H3.704l1.048-.943a.75.75 0 1 0-1.004-1.114l-2.5 2.25a.75.75 0 0 0 0 1.114l2.5 2.25a.75.75 0 1 0 1.004-1.114l-1.048-.943h9.546A.75.75 0 0 0 14 10Z" clipRule="evenodd" />
                                    </svg>

                                    <span className="is-drawer-close:hidden">Logout</span>
                                </button>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </>
    )
}