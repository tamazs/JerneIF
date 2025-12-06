import {Link} from "react-router";

// @ts-ignore
export default function Card({ title, description, linkText, linkTo }) {

    return (
        <div className="card card-border bg-secondary text-primary">
            <div className="card-body">
                <h2 className="card-title">{title}</h2>
                <p>{description}</p>
                <div className="card-actions justify-end">
                    <Link to={linkTo}>{linkText}</Link>
                </div>
            </div>
        </div>
    )
}