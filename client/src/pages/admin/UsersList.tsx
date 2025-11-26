export default function UsersList() {
    return (
        <div className="overflow-x-auto flex flex-col items-center">
            <table className="table">
                {/* head */}
                <thead>
                <tr>
                    <th></th>
                    <th>Name</th>
                    <th>Job</th>
                    <th>Favorite Color</th>
                    <th>Active</th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
                {/* row 1 */}
                <tr className="hover:bg-base-300">
                    <th>1</th>
                    <td>Cy Ganderton</td>
                    <td>Quality Control Specialist</td>
                    <td>Blue</td>
                    <td>
                        <div className="badge badge-secondary">Active</div>
                    </td>
                    <td>
                        <div className="dropdown dropdown-left">
                            <div tabIndex={0} role="button" className="btn"><svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="size-6">
                                <path strokeLinecap="round" strokeLinejoin="round" d="M6.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0ZM12.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0ZM18.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Z" />
                            </svg>
                            </div>
                            <ul tabIndex={-1} className="dropdown-content menu bg-base-100 rounded-box z-1 w-52 p-2 shadow-sm">
                                <li><a>Edit User</a></li>
                                <li><a>Delete User</a></li>
                            </ul>
                        </div>
                    </td>
                </tr>
                {/* row 2 */}
                <tr className="hover:bg-base-300">
                    <th>2</th>
                    <td>Hart Hagerty</td>
                    <td>Desktop Support Technician</td>
                    <td>Purple</td>
                    <td>
                        <div className="badge badge-secondary">Active</div>
                    </td>
                    <td>
                        <div className="dropdown dropdown-left">
                            <div tabIndex={0} role="button" className="btn"><svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="size-6">
                                <path strokeLinecap="round" strokeLinejoin="round" d="M6.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0ZM12.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0ZM18.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Z" />
                            </svg>
                            </div>
                            <ul tabIndex={-1} className="dropdown-content menu bg-base-100 rounded-box z-1 w-52 p-2 shadow-sm">
                                <li><a>Edit User</a></li>
                                <li><a>Delete User</a></li>
                            </ul>
                        </div>
                    </td>
                </tr>
                {/* row 3 */}
                <tr className="hover:bg-base-300">
                    <th>3</th>
                    <td>Brice Swyre</td>
                    <td>Tax Accountant</td>
                    <td>Red</td>
                    <td>
                        <div className="badge badge-error">Inactive</div>
                    </td>
                    <td>
                        <div className="dropdown dropdown-left">
                            <div tabIndex={0} role="button" className="btn"><svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="size-6">
                                <path strokeLinecap="round" strokeLinejoin="round" d="M6.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0ZM12.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0ZM18.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Z" />
                            </svg>
                            </div>
                            <ul tabIndex={-1} className="dropdown-content menu bg-base-100 rounded-box z-1 w-52 p-2 shadow-sm">
                                <li><a>Edit User</a></li>
                                <li><a>Delete User</a></li>
                            </ul>
                        </div>
                    </td>
                </tr>
                </tbody>
            </table>
            <div className="join mt-6">
                <button className="join-item btn btn-md">1</button>
                <button className="join-item btn btn-md btn-active">2</button>
                <button className="join-item btn btn-md">3</button>
                <button className="join-item btn btn-md">4</button>
            </div>
        </div>
    )
}