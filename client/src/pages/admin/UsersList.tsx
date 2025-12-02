import {useEffect, useState} from "react";
import useJerneCrud from "../../hooks/useJerneCrud.ts";
import {useAtom} from "jotai";
import {usersAtom} from "../../atoms/jerneAtom.ts";
import {useNavigate} from "react-router";

export default function UsersList() {
    const [search, setSearch] = useState("");
    const [role, setRole] = useState("");
    const [active, setActive] = useState("");
    const [page, setPage] = useState(1);
    const navigate = useNavigate();

    const [users] = useAtom(usersAtom);

    const jerneCrud = useJerneCrud();

    function buildSieveModel() {
        const filters = [];

        if (search) filters.push(`fullName@=*${search}`);
        if (role) filters.push(`role==${role}`);
        if (active) filters.push(`isActive==${active}`);

        return {
            filters: filters.join(","),
            sorts: "-fullName",
            page,
            pageSize: 5
        };
    }

    useEffect(() => {
        const sieve = buildSieveModel();
        jerneCrud.getAllUsers(sieve);
    }, [search, role, active, page]);


    return (
        <div className="overflow-x-auto flex flex-col items-center">
            <label className="input mb-3">
                <svg className="h-[1em] opacity-50" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                    <g
                        strokeLinejoin="round"
                        strokeLinecap="round"
                        strokeWidth="2.5"
                        fill="none"
                        stroke="currentColor"
                    >
                        <circle cx="11" cy="11" r="8"></circle>
                        <path d="m21 21-4.3-4.3"></path>
                    </g>
                </svg>
                <input type="search" required placeholder="Search" onChange={(e) => setSearch(e.target.value)} />
            </label>
            <div className="flex w-full justify-start items-center gap-12 mb-4">
                <form className="filter">
                    <input className="btn btn-square" type="reset" value="×" onClick={() => setRole("")}/>
                    <input className="btn" type="radio" name="frameworks" aria-label="Admin" onClick={() => setRole("Admin")}/>
                    <input className="btn" type="radio" name="frameworks" aria-label="Player" onClick={() => setRole("Player")}/>
                </form>
                <form className="filter">
                    <input className="btn btn-square" type="reset" value="×" onClick={() => setActive("")}/>
                    <input className="btn" type="radio" name="frameworks" aria-label="Active" onClick={() => setActive("true")}/>
                    <input className="btn" type="radio" name="frameworks" aria-label="Inactive" onClick={() => setActive("false")}/>
                </form>
            </div>
            <table className="table">
                {/* head */}
                <thead>
                <tr>
                    <th>Name</th>
                    <th>Phone Number</th>
                    <th>Email</th>
                    <th>Role</th>
                    <th>Active</th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
                {users.map((user) => {
                    return (
                        <tr className="hover:bg-base-300" key={user.userId}>
                            <td>{user.fullName}</td>
                            <td>{user.phoneNumber}</td>
                            <td>{user.email}</td>
                            <td>{user.role}</td>
                            <td>
                                {user.isActive ? <div className="badge badge-secondary">Active</div> : <div className="badge badge-error">Inactive</div>}
                            </td>
                            <td>
                                <div className="dropdown dropdown-left">
                                    <div tabIndex={0} role="button" className="btn"><svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="size-6">
                                        <path strokeLinecap="round" strokeLinejoin="round" d="M6.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0ZM12.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0ZM18.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Z" />
                                    </svg>
                                    </div>
                                    <ul tabIndex={-1} className="dropdown-content menu bg-base-100 rounded-box z-1 w-52 p-2 shadow-sm">
                                        <li><a onClick={() => {
                                            navigate(`/edituser/${user.userId}`);
                                        }}>Edit User</a></li>
                                        <li><a onClick={() => jerneCrud.deleteUser(user.userId)}>Delete User</a></li>
                                    </ul>
                                </div>
                            </td>
                        </tr>
                    )
                })}
                </tbody>
            </table>
            <div className="join grid grid-cols-2">
                <button className="join-item btn btn-outline" onClick={() => setPage(page - 1)} disabled={page === 1}>Previous page</button>
                <button className="join-item btn btn-outline" onClick={() => setPage(page + 1)}>Next</button>
            </div>
        </div>
    )
}