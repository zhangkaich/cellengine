
/* 
 * Copyright 2001-2009 James House 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not 
 * use this file except in compliance with the License. You may obtain a copy 
 * of the License at 
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0 
 *   
 * Unless required by applicable law or agreed to in writing, software 
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations 
 * under the License.
 * 
 */

/*
 * Previously Copyright (c) 2001-2004 James House
 */
package org.quartz;

/**
 * <p>
 * A marker interface for <code>{@link org.quartz.JobDetail}</code> s that
 * wish to have their state maintained between executions.
 * </p>
 * 
 * <p>
 * <code>StatefulJob</code> instances follow slightly different rules from
 * regular <code>Job</code> instances. The key difference is that their
 * associated <code>{@link JobDataMap}</code> is re-persisted after every
 * execution of the job, thus preserving state for the next execution. The
 * other difference is that stateful jobs are not allowed to execute
 * concurrently, which means new triggers that occur before the completion of
 * the <code>execute(xx)</code> method will be delayed.
 * </p>
 * 
 * @see Job
 * @see JobDetail
 * @see JobDataMap
 * @see Scheduler
 * 
 * @author James House
 */
public interface StatefulJob extends Job {

    /*
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * 
     * Interface.
     * 
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     */

}
