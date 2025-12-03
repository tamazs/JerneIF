import {useEffect, useState} from "react";
import useJerneCrud from "../hooks/useJerneCrud.ts";
import {useAtom} from "jotai";
import {useNavigate} from "react-router";
import {transactionsAtom} from "../atoms/jerneAtom.ts";

export default function TransactionsPage() {
    const [search, setSearch] = useState("");
    const [status, setStatus] = useState("");
    const [page, setPage] = useState(1);
    const navigate = useNavigate();

    const [transactions] = useAtom(transactionsAtom);

    const jerneCrud = useJerneCrud();

    function buildSieveModel() {
        const filters = [];

        if (search) filters.push(`fullName@=*${search}`);
        if (status) filters.push(`status==${status}`);

        return {
            filters: filters.join(","),
            sorts: "-createdAt",
            page,
            pageSize: 5
        };
    }

    useEffect(() => {
        const sieve = buildSieveModel();
        jerneCrud.getTransactions(sieve);
    }, [search, status, page]);


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
                    <input className="btn btn-square" type="reset" value="×" onClick={() => setStatus("")}/>
                    <input className="btn" type="radio" name="frameworks" aria-label="Pending" onClick={() => setStatus("Pending")}/>
                    <input className="btn" type="radio" name="frameworks" aria-label="Approved" onClick={() => setStatus("Approved")}/>
                    <input className="btn" type="radio" name="frameworks" aria-label="Rejected" onClick={() => setStatus("Rejected")}/>
                </form>
            </div>
            <table className="table mb-4">
                {/* head */}
                <thead>
                <tr>
                    <th>Name</th>
                    <th>MobilePay Reference</th>
                    <th>Amount</th>
                    <th>Status</th>
                    <th>Created At</th>
                    <th></th>
                </tr>
                </thead>
                <tbody>
                {transactions.map((transaction) => {
                    return (
                        <tr className="hover:bg-base-300" key={transaction.transactionId}>
                            <td>{transaction.userFullName}</td>
                            <td>{transaction.mobilePayReference}</td>
                            <td>{transaction.amount} DKK</td>
                            <td>
                                {transaction.status === "Pending" && (
                                    <div className="badge badge-warning">Pending</div>
                                )}
                                {transaction.status === "Approved" && (
                                    <div className="badge badge-success">Approved</div>
                                )}
                                {transaction.status === "Rejected" && (
                                    <div className="badge badge-error">Rejected</div>
                                )}
                            </td>
                            <td>{new Date(transaction.createdAt).toLocaleString("da-DK", {
                                timeZone: "Europe/Copenhagen",
                                year: "numeric",
                                month: "2-digit",
                                day: "2-digit",
                                hour: "2-digit",
                                minute: "2-digit",
                            })}</td>
                            <td>
                                {transaction.status === "Pending" && (
                                    <div className="dropdown dropdown-left">
                                        <div tabIndex={0} role="button" className="btn"><svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="size-6">
                                            <path strokeLinecap="round" strokeLinejoin="round" d="M6.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0ZM12.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0ZM18.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Z" />
                                        </svg>
                                        </div>
                                        <ul tabIndex={-1} className="dropdown-content menu bg-base-100 rounded-box z-1 w-52 p-2 shadow-sm">
                                            <li><a >Approve Transaction</a></li>
                                            <li><a >Reject Transaction</a></li>
                                        </ul>
                                    </div>
                                )}
                            </td>
                        </tr>
                    )
                })}
                </tbody>
            </table>
            <div className="join grid grid-cols-2 gap-4">
                <button className="join-item btn btn-outline" onClick={() => setPage(page - 1)} disabled={page === 1}>Previous page</button>
                <button className="join-item btn btn-outline" onClick={() => setPage(page + 1)}>Next page</button>
            </div>
        </div>
    )
}